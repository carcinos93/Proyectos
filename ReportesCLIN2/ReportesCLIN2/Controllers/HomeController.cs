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
using System.Collections.Specialized;

namespace ReportesCLIN2.Controllers
{
    using Microsoft.Reporting.WebForms;
    using Models;
    using System.Net;
    using System.Security.Principal;
    using System.Web.UI.WebControls;

    public class icustome : IReportServerCredentials
    {
        public WindowsIdentity ImpersonationUser { get; set; }

        public ICredentials NetworkCredentials { get { return new System.Net.NetworkCredential("nelso", "Nightmare1"); } }

        public bool GetFormsCredentials(out Cookie authCookie, out string userName, out string password, out string authority)
        {
            authCookie = null;
            userName = "";
            password = "";
            authority = "";
            return false;
        }
    }
    public class HomeController : Controller
    {
        
        public ActionResult Index()
        {
            var xml = XDocument.Load(Server.MapPath("~/Reports/Reportes.xml"));
            IList<Reporte> reportes = new List<Reporte>();
            if (xml != null)
            {
                XNamespace ns = xml.Root.GetDefaultNamespace();
                var x = xml.Descendants().Where(q => q.Name.LocalName == "Reportes").FirstOrDefault();
                Reports.Controls controls = new Reports.Controls();
                x.Descendants().ToList().ForEach((e) =>
               {
                   reportes.Add(new Reporte
                   {
                       name = e.Attribute("name").Value,
                       text = e.Attribute("text").Value,
                       description = e.Value
                   });
               });
            }

            //ViewBag.Reportes = reportes;

            return View(reportes);
        }
        [HttpGet]
        public ActionResult ReportViewer()
        {
            var reporte = Request.Params["reporte"];
            var reportViewer = new ReportViewer()
            {
                ProcessingMode = ProcessingMode.Remote,
                SizeToReportContent = false,
                Width = Unit.Percentage(100),
                Height = Unit.Pixel(900),
                
            };

            reportViewer.ServerReport.ReportPath = "/Reportes CLIN/" + reporte;
            reportViewer.ServerReport.ReportServerUrl = new Uri("http://laptop-0hm5b1ni:10110/ReportServer/");
            reportViewer.ServerReport.ReportServerCredentials = new icustome(); ;


            ViewBag.ReportViewer = reportViewer;

            return View("ssrsViewer");
        }

        public ActionResult NotFound()
        {
                return View("NotFound");
        }

       


        public FileResult ExportReport(string reporte, string parametros)
        {
            string param;
            if (Request.Params["parametros"] == null)
            {
                Dictionary<string, object> p = new Dictionary<string, object>();

                foreach (string i in Request.QueryString.AllKeys)
                {
                    p.Add(i, Request.QueryString.Get(i));
                }

                param = Newtonsoft.Json.JsonConvert.SerializeObject(p);
            }

            else
                param = Request.Params["parametros"];

            string formato = Request.Params["formato"];
            Reporting.RenderFormat formatoExport = new Reporting.RenderFormat(formato);



            string extension = formatoExport.extension;

            if (!Reporting.ExistsReport(reporte))
                this.RedirectToAction("NotFound");



            return File(Reporting.Export(reporte, param, formatoExport.formato), "application/octet-stream", reporte + "." + extension);

        }

        
        public ActionResult RenderReport(string reporte)
        {
            string parametros;
            if (Request.Params["parametros"] == null)
            {
                Dictionary<string, object> p = new Dictionary<string, object>();
          
                    foreach (string i in Request.QueryString.AllKeys)
                    {
                        p.Add(i, Request.QueryString.Get(i));
                    }
                
                parametros = Newtonsoft.Json.JsonConvert.SerializeObject( p );
            }
               
            else
                parametros = Request.Params["parametros"];

            if (!Reporting.ExistsReport(reporte))
                return NotFound();


            ViewBag.DataHTML = Reporting.Render(reporte, parametros, Reporting.EnumRenderFormat.HTML );

            return View("RenderViewer");
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
