using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Linq;
using System.Web.Mvc;
using Dapper;
using DotLiquid;
using HtmlToOpenXml;
using System.IO;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using TheArtOfDev.HtmlRenderer;
using TheArtOfDev;
using PdfSharp.Pdf;

namespace ReportesCLIN2.Controllers
{
    public class Reporting
    {
        public enum RenderFormat
        {
            PDF = 1,
            WORD = 2
        }
        private static System.Data.DbType GetType(string type)
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

        public static bool ExistsReport(string reporte)
        {
            return (!string.IsNullOrWhiteSpace(reporte) & System.IO.File.Exists(HttpContext.Current.Server.MapPath("~/Reports/" + reporte + ".html")) & System.IO.File.Exists(HttpContext.Current.Server.MapPath("~/Reports/" + reporte + ".data")));

        }

        public static byte[] Export(string reporte, string parametros, RenderFormat format)
        {
            string html = Render(reporte, parametros);
            byte[] data = null;
            if (format == RenderFormat.WORD)
            {
                using (MemoryStream generatedDocument = new MemoryStream())
                {
                    using (WordprocessingDocument package = WordprocessingDocument.Create(generatedDocument, WordprocessingDocumentType.Document))
                    {
                        MainDocumentPart mainPart = package.MainDocumentPart;

                        if (mainPart == null)
                        {
                            mainPart = package.AddMainDocumentPart();
                            new DocumentFormat.OpenXml.Wordprocessing.Document(new Body()).Save(mainPart);

                        }

                        HtmlConverter converter = new HtmlConverter(mainPart);
                        converter.ParseHtml(html);

                        mainPart.Document.Save();
                    }

                    /*File.WriteAllBytes("archivoprueba.docx", generatedDocument.ToArray());*/
                    data = generatedDocument.ToArray();
                }
            }
            if (format  == RenderFormat.PDF)
            {
                TheArtOfDev.HtmlRenderer.PdfSharp.PdfGenerateConfig config = new TheArtOfDev.HtmlRenderer.PdfSharp.PdfGenerateConfig();

                config.PageSize = PdfSharp.PageSize.A4;
                config.MarginBottom = 60;
                config.MarginLeft = 30;
                config.MarginRight = 30;
                config.MarginTop = 60;
              
                PdfDocument pdf = TheArtOfDev.HtmlRenderer.PdfSharp.PdfGenerator.GeneratePdf(html, config );
                using (MemoryStream stream  = new MemoryStream())
                {
                    pdf.Save(stream, true);
                    data = stream.ToArray();
                }
                   

            }
            return data;
        }
            public static string Render(string reporte,string parametros)
        {


            DotLiquid.Template.RegisterFilter(typeof(Reports.LiquidFilters));


            var obj_parametros = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(parametros);

            string texto = System.IO.File.ReadAllText(HttpContext.Current.Server.MapPath("~/Reports/" + reporte + ".html"));

            var xml = XDocument.Load(HttpContext.Current.Server.MapPath("~/Reports/" + reporte + ".data"));
            Dictionary<string, object> objeto = new Dictionary<string, object>();
            if (xml != null)
            {
                XNamespace ns = xml.Root.GetDefaultNamespace();
                var x = xml.Descendants().Where(q => q.Name.LocalName == "DataSets").FirstOrDefault();
                x.Elements().ToList().ForEach((e) =>
                {
                    bool IsMultiRecords = e.Attribute("IsMultiRecords") != null ? e.Attribute("IsMultiRecords").Value == "1" : false;
                    string Type = e.Attribute("type") == null ? "SQL" : e.Attribute("type").Value;
                    string Name = e.Attribute("name").Value;
                    string SQL = e.Element(e.GetDefaultNamespace() + "SQL").Value;
                    using (var conn = DbFactory.Conn())
                    {
                        conn.Open();

                        DynamicParameters parameters = new DynamicParameters();
                        e.Element(e.GetDefaultNamespace() + "Parameters").Elements().ToList().ForEach((a) =>
                        {
                            string parameterName = a.Attribute("name").Value;
                            object value = obj_parametros[parameterName];
                            System.Data.DbType dbType = GetType(a.Attribute("type") == null ? "string" : a.Attribute("type").Value);
                            parameters.Add(parameterName, value, dbType);

                        });
                        if (IsMultiRecords)
                        {
                            List<object> datos = new List<object>();
                            conn.Query(SQL, parameters).ToList().ForEach((f) =>
                            {
                                datos.Add(DapperHelpers.ToExpandoObject(f));
                            });
                            objeto.Add(Name, datos);
                        }

                        else
                        {
                            var firstOrDefault = conn.Query(SQL, parameters).FirstOrDefault();
                            objeto.Add(Name, firstOrDefault);
                        }
                    }
                });
            }
            DotLiquid.Template template = DotLiquid.Template.Parse(texto);
            string html = template.Render(Hash.FromDictionary(objeto));
            return html;
        }
    }
}