using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReportesCLIN2.Reports
{
    public static class LiquidFilters
    {
        public static string Format(object input, string format)
        {
            if (input == null)
                return null;
            else if (string.IsNullOrWhiteSpace(format))
                return input.ToString();

            return string.Format("{0:" + format + "}", input);
        }
    }
}
