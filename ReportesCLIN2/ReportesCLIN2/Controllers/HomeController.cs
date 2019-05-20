using DotLiquid;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Xml.Linq;

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
            ViewBag.Reporte = reporte;
            if (string.IsNullOrWhiteSpace(reporte))
                return View("NotFound");

            if (!System.IO.File.Exists(Server.MapPath("~/Reports/" + reporte + ".data")))
                return View("NotFound");

            string texto = System.IO.File.ReadAllText(Server.MapPath("~/Reports/" + reporte + ".html"));

            DotLiquid.Template template = DotLiquid.Template.Parse(texto);
            string html = template.Render(Hash.FromAnonymousObject(new
            {
                deudor = "Nelson Alfonso Rodas Villalta",
                dui = "04754407-4",
                title = reporte,
                user = new { name = "nrodas" },
                products = new object[] { new { name = "Meet", price = 1999.00, fecha = DateTime.Now }, new { name = "Cheese", price = 1.45, fecha = DateTime.Now } }
            }));

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