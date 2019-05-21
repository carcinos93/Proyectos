using DotLiquid;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Xml.Linq;
using Dapper;
using System.Dynamic;

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
            DotLiquid.Template.RegisterFilter(typeof(Reports.LiquidFilters));
            string parametros = Request.Params["parametros"];
            
            var obj_parametros = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(parametros);
            
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
                    bool IsMultiRecords = e.Attribute("IsMultiRecords") != null ? e.Attribute("IsMultiRecords").Value == "1" : false;
                    string Type = e.Attribute("type") == null ? "SQL" : e.Attribute("type").Value;
                    string Name = e.Attribute("name").Value;
                    string SQL = e.Element(e.GetDefaultNamespace() + "SQL").Value;
                    using (var conn = DbFactory.Conn())
                    {
                        conn.Open();

                        DynamicParameters parameters = new DynamicParameters();
                        e.Element(e.GetDefaultNamespace() + "Parameters").Elements().ToList().ForEach((a) => {
                            string parameterName = a.Attribute("name").Value;
                            object value = obj_parametros[parameterName];
                            System.Data.DbType dbType = GetType(a.Attribute("type") == null ? "string" : a.Attribute("type").Value);
                            parameters.Add(parameterName, value, dbType);

                        });
                        if (IsMultiRecords)
                        {

                            List<object> datos = new List<object>();
                             conn.Query(SQL, parameters).ToList().ForEach((f) => {
                                 datos.Add( DapperHelpers.ToExpandoObject(f) );
                            });

                           /* var dato = Newtonsoft.Json.JsonConvert.SerializeObject( new { collection = conn.Query(SQL, parameters).Select(i => DapperHelpers.ToExpandoObject(i)) });

                            var json = Newtonsoft.Json.JsonConvert.DeserializeObject<IDictionary<string, object>>(dato, new DictionaryConverter());*/
                            objeto.Add(Name, datos);
                        }
                            
                        else
                        {
                            var firstOrDefault = conn.Query(SQL, parameters).FirstOrDefault();
                             objeto.Add(Name, firstOrDefault);
                        }
                            

       
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
        private System.Data.DbType GetType(string type)
        {
            switch (type.ToLower())
            {
                case "varchar":
                    return System.Data.DbType.String;
                case "int32":
                    return System.Data.DbType.Int32;
                case "single":
                    return System.Data.DbType.Single;
                case "datetime":
                    return System.Data.DbType.DateTime;
                case "datetime2":
                    return System.Data.DbType.DateTime2;
                case "double":
                    return System.Data.DbType.Double;
                case "boolean":
                    return System.Data.DbType.Boolean;
                default:
                    return System.Data.DbType.Object;
            }
        }

    }
}