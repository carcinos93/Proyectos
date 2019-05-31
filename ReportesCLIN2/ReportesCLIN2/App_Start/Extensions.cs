using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ReportesCLIN2
{
    public static class Extensiones
    {
        public static string HtmlCaracters(this string html)
        {
            return html.Replace("á", "&eacute;")
                .Replace("é", "&eacute;")
                .Replace("í", "&iacute;")
                .Replace("ó", "&oacute;")
                .Replace("ú", "&uacute;")
                .Replace("Á", "&Aacute;")
                .Replace("É", "&Eacute;")
                .Replace("Í", "&Iacute;")
                .Replace("Ó", "&Oacute;")
                .Replace("Ú", "&Uacute;")
                .Replace("ñ", "&ntilde;")
                .Replace("Ñ", "&Ntilde;")
                .Replace("@","&commat;")
                .Replace("“", "&#x201C;")
                .Replace("”", "&#x201D;");

                
        }
    }
}