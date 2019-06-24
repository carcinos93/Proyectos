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
using System.Diagnostics;

namespace ReportesCLIN2.Controllers
{
    public class Reporting
    {
        public class RenderFormat
        {
            public RenderFormat(string renderFormat)
            {

                switch (renderFormat)
                {
                    case "pdf":
                        extension = "pdf";
                        formato = EnumRenderFormat.PDF;
                        break;
                    case "word":
                        extension = "docx";
                        formato = EnumRenderFormat.WORD;
                        break;
                    case "html":
                        extension = "html";
                        formato = EnumRenderFormat.HTML;
                        break;
                    default:
                        break;
                }
            }
            public EnumRenderFormat formato;
            public string extension;


        }




        public enum EnumRenderFormat
        {
            PDF = 1,
            WORD = 2,
            HTML = 3
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

        public static byte[] Export(string reporte, string parametros, EnumRenderFormat format)
        {
            string html = Render(reporte, parametros, format);
            Dictionary<string, string> propiedades = new Dictionary<string, string>();
            var xml = XDocument.Load(HttpContext.Current.Server.MapPath("~/Reports/" + reporte + ".data"));
            PageNumberType pageNumber = new PageNumberType()
            {
                ChapterSeparator = ChapterSeparatorValues.Colon,
                Start = 1,
                Format = NumberFormatValues.Decimal
            };

            XNamespace ns = xml.Root.GetDefaultNamespace();
            var x = xml.Descendants().Where(q => q.Name.LocalName == "Properties").FirstOrDefault();
            if (x != null)
            {
                x.Elements().ToList().ForEach((e) =>
              {
                  propiedades.Add(e.Attribute("name").Value, e.Attribute("value").Value);

              });
            }
            byte[] data = null;
            if (format == EnumRenderFormat.WORD)
            {
                using (MemoryStream generatedDocument = new MemoryStream())
                {

                    string template = xml.Root.Attribute("template") == null ? null : xml.Root.Attribute("template").Value;

                    using (var templateFile = File.Open(HttpContext.Current.Server.MapPath("~/templates/" + template), FileMode.Open, FileAccess.Read))
                    {
                  
                        templateFile.CopyTo(generatedDocument);
                    }

                    using (WordprocessingDocument package = WordprocessingDocument.Open(generatedDocument, true))
                    {
                        MainDocumentPart mainPart = package.MainDocumentPart;
                        SectionProperties sectionProps = new SectionProperties();

                        Int32 top = (int)((propiedades.ContainsKey("margin-top") ? int.Parse(propiedades["margin-top"].ToString()) : 0) * 56.7);
                        Int32 bottom = (int)((propiedades.ContainsKey("margin-bottom") ? int.Parse(propiedades["margin-bottom"].ToString()) : 0) * 56.7);
                        UInt32 left = (uint)((propiedades.ContainsKey("margin-left") ? int.Parse(propiedades["margin-left"].ToString()) : 0) * 56.7);
                        UInt32 right = (uint)((propiedades.ContainsKey("margin-right") ? int.Parse(propiedades["margin-right"].ToString()) : 0) * 56.7);



                        PageMargin pageMargin = new PageMargin() { Top = top, Bottom = bottom, Left = left, Right = right };

                        if (mainPart == null)
                        {
                           
                            mainPart = package.AddMainDocumentPart();
                            new DocumentFormat.OpenXml.Wordprocessing.Document(new Body()).Save(mainPart);



                        }
                        sectionProps.Append(pageMargin);
                        mainPart.Document.Body.Append(sectionProps);
                        HtmlConverter converter = new HtmlConverter(mainPart);
                        converter.ParseHtml(html);
                        mainPart.Document.Save();
                    }

                    /*File.WriteAllBytes("archivoprueba.docx", generatedDocument.ToArray());*/
                    data = generatedDocument.ToArray();
                }
            }
            if (format == EnumRenderFormat.PDF)
            {
                var nombreArchivo = string.Format("{0}", Guid.NewGuid().ToString());
                var rutaHtml = HttpContext.Current.Server.MapPath("~/temp/" + nombreArchivo + ".html");
                File.AppendAllText(rutaHtml, html, System.Text.Encoding.UTF8);
                byte[] salida = { 0x10 };
                using (Process p = new Process())
                {
                    var proc1 = new ProcessStartInfo();
                    string props = string.Join(" ", propiedades.Select(e => string.Format("--{0} {1}", e.Key, e.Value)));

                    proc1.UseShellExecute = false;
                    proc1.WorkingDirectory = HttpContext.Current.Server.MapPath("~/");
                    proc1.FileName = HttpContext.Current.Server.MapPath("~/wkhtmltopdf.exe");
                    //proc1.Verb = "toc";
                    proc1.Arguments = "" + props + " " + rutaHtml + " " + ("temp/" + nombreArchivo + ".pdf");
                    proc1.WindowStyle = ProcessWindowStyle.Hidden;
                    p.StartInfo = proc1;
                    p.Start();
                    do
                    {
                        if (p.HasExited)
                            break;

                    } while (!p.HasExited);
                    data = File.ReadAllBytes(HttpContext.Current.Server.MapPath("~/temp/" + nombreArchivo + ".pdf"));

                }




                /*TheArtOfDev.HtmlRenderer.PdfSharp.PdfGenerateConfig config = new TheArtOfDev.HtmlRenderer.PdfSharp.PdfGenerateConfig();

                config.PageSize = PdfSharp.PageSize.A4;
                config.MarginBottom = 60;
                config.MarginLeft = 60;
                config.MarginRight = 60;
                config.MarginTop = 60;
              
                PdfDocument pdf = TheArtOfDev.HtmlRenderer.PdfSharp.PdfGenerator.GeneratePdf(html, config );
                using (MemoryStream stream  = new MemoryStream())
                {
                    pdf.Save(stream, true);
                    data = stream.ToArray();
                }*/


            }
            if (format == EnumRenderFormat.HTML)
            {
                data = System.Text.Encoding.ASCII.GetBytes(html);
            }

            return data;
        }


        public static string Render(string reporte, string parametros, EnumRenderFormat format)
        {

            System.Globalization.CultureInfo ci = new System.Globalization.CultureInfo("es-ES");
            System.Threading.Thread.CurrentThread.CurrentCulture = ci;
            DotLiquid.Template.RegisterFilter(typeof(Reports.LiquidFilters));


            var obj_parametros = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(parametros);

            string texto = System.IO.File.ReadAllText(HttpContext.Current.Server.MapPath("~/Reports/" + reporte + ".html"));

            var xml = XDocument.Load(HttpContext.Current.Server.MapPath("~/Reports/" + reporte + ".data"));
            Dictionary<string, object> objeto = new Dictionary<string, object>();
            if (xml != null)
            {
                XNamespace ns = xml.Root.GetDefaultNamespace();
                var x = xml.Descendants().Where(q => q.Name.LocalName == "DataSets").FirstOrDefault();
                string strconn = xml.Descendants().Where(q => q.Name.LocalName == "DataSource").FirstOrDefault().Value;
                x.Elements().ToList().ForEach((e) =>
                {
                    bool IsMultiRecords = e.Attribute("IsMultiRecords") != null ? e.Attribute("IsMultiRecords").Value == "1" : false;
                    string Type = e.Attribute("type") == null ? "query" : e.Attribute("type").Value;
                    string Name = e.Attribute("name").Value;
                    string SQL = e.Element(e.GetDefaultNamespace() + "SQL").Value;

                    System.Data.CommandType commandType = Type == "query" ? System.Data.CommandType.Text : System.Data.CommandType.StoredProcedure;
                    using (var conn = DbFactory.Conn(strconn))
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
                            conn.Query(sql: SQL, param: parameters, commandType: commandType).ToList().ForEach((f) =>
                            {
                                datos.Add(DapperHelpers.ToExpandoObject(f));
                            });
                            objeto.Add(Name, datos);
                        }

                        else
                        {
                            var firstOrDefault = conn.Query(sql: SQL, param: parameters, commandType: commandType).FirstOrDefault();
                            objeto.Add(Name, firstOrDefault);
                        }
                    }
                });
            }
            objeto.Add("RenderFormat", format.ToString());
            objeto.Add("Today", DateTime.Now);
            DotLiquid.Template template = DotLiquid.Template.Parse(texto);
            string html = template.Render(Hash.FromDictionary(objeto));
            return html;
        }
    }
}