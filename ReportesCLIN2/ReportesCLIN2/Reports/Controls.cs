using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ReportesCLIN2.Reports
{
    public class Controls
    {
        private List<string> resultado { get; set; }
        public Controls()
        {
            this.resultado = new List<string>();
        }
        public void AddDate(dynamic Properties)
        {
            string control = @"<div class='form-group'><label>{1}</label>&nbsp;<input class='param form-control' type='date' name='{0}'/></div>";
            this.resultado.Add(string.Format(control, Properties.name, Properties.label));
        }
        public void AddTextBox(dynamic Properties)
        {
            string control = @"<div class='form-group'><label>{1}</label>&nbsp;<input class='param form-control' type='textbox' name='{0}'/></div>";
            this.resultado.Add(string.Format(control, Properties.name, Properties.label));
        }

       public string Render()
        {
            string render = "";
            foreach (var item in this.resultado)
            {
                render += string.Format("<div>{0}</div>", item);
            }

            return render;
        }
        
    }
}