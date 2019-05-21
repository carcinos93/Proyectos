using DotLiquid;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Xml.Linq;
using Dapper;
namespace ReportesCLIN2.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public ActionResult RenderReport(string reporte)
        {
            string parametros = Request.Params["parametros"];
            
            var obj_parametros = Newtonsoft.Json.JsonConvert.DeserializeObject(parametros, typeof(Dictionary<string, object>));
            
            ViewBag.Reporte = reporte;
            if (string.IsNullOrWhiteSpace(reporte))
                return View("NotFound");

            if (!System.IO.File.Exists(Server.MapPath("~/Reports/" + reporte + ".data")))
                return View("NotFound");

            string texto = System.IO.File.ReadAllText(Server.MapPath("~/Reports/" + reporte + ".html"));

            var xml = XDocument.Load(Server.MapPath("~/Reports/" + reporte + ".data"));
            Dictionary<string, object> objeto = new Dictionary<string, object>();
            if (xml != null)
            {
                XNamespace ns = xml.Root.GetDefaultNamespace();
                var x = xml.Descendants().Where(q => q.Name.LocalName == "DataSets").FirstOrDefault();
                x.Elements().ToList().ForEach((e) => {
                    bool IsMultiRecords = e.Attribute("IsMultiRecords") == null ? e.Attribute("IsMultiRecords").Value == "1" : false;
                    string Type = e.Attribute("Type") == null ? "SQL" : e.Attribute("Type").Value;
                    string SQL = e.Element(e.GetDefaultNamespace() + "SQL").Value;
                    using (var conn = DbFactory.Conn())
                    {
                        conn.Open();

                        var datos = conn.Query(SQL, obj_parametros).ToList()[0];
                        objeto.Add("dato", datos);
                    }

                        int test = 0;
                });
            }
            DotLiquid.Template template = DotLiquid.Template.Parse(texto);
            string html = template.Render(Hash.FromDictionary(objeto));

            ViewBag.DataHTML = html;

            return View("ReportViewer");
        }
        [HttpGet()]
        public ActionResult ViewReport(string reporte)
        {
             ViewBag.Reporte = reporte;
            var xml = XDocument.Load(Server.MapPath("~/Reports/" + reporte + ".data"));
            if (xml != null)
            {
                XNamespace ns = xml.Root.GetDefaultNamespace();
                var x = xml.Descendants().Where(q => q.Name.LocalName == "Params").FirstOrDefault();
                Reports.Controls controls = new Reports.Controls();
                x.Descendants().ToList().ForEach((e) =>
               {
                   var type = e.Attribute("type").Value ?? "textbox";
                   var name = e.Attribute("name").Value;
                   var label = e.Attribute("label").Value == null ? e.Attribute("name").Value.Replace('_', ' ') : e.Attribute("label").Value;

                   if (type == "textbox")
                       controls.AddTextBox(new { label = label, name = name });
                   if (type == "date")
                       controls.AddDate(new { label = label, name = name });
               });

                ViewBag.Parametros = controls.Render();
            }

            return View("ReportViewer");
        }

    }
}