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
        /// <summary>
        /// Agrega control tipo fecha
        /// </summary>
        /// <param name="Properties"></param>
        public void AddDate(dynamic Properties)
        {
            string control = @"<div class='form-group'><label>{1}</label>&nbsp;<input class='param form-control' type='date' name='{0}'/></div>";
            this.resultado.Add(string.Format(control, Properties.name, Properties.label));
        }
        /// <summary>
        /// Agrega control tipo texto
        /// </summary>
        /// <param name="Properties"></param>
        public void AddTextBox(dynamic Properties)
        {
            string control = @"<div class='form-group'><label>{1}</label>&nbsp;<input class='param form-control' type='textbox' name='{0}' value='{2}'/></div>";
            this.resultado.Add(string.Format(control, Properties.name, Properties.label, Properties.value));
        }
        /// <summary>
        /// Retorna todos los controles 
        /// </summary>
        /// <returns></returns>
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