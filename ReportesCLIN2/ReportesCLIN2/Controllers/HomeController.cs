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
        public ActionResult NotFound()
        {
                return View("NotFound");
        }
        public FileResult ExportReport(string reporte, string parametros)
        {
            if (!Reporting.ExistsReport(reporte))
                this.RedirectToAction("NotFound");



            return File(Reporting.Export(reporte, parametros, Reporting.RenderFormat.PDF), "application/octet-stream", reporte + ".pdf");

        }

        [HttpPost]
        public ActionResult RenderReport(string reporte)
        {

           string parametros = Request.Params["parametros"];
            ViewBag.Reporte = reporte;
            if (!Reporting.ExistsReport(reporte))
                return NotFound();


            ViewBag.DataHTML = Reporting.Render(reporte, parametros);

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
