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
            if (format.ToLower() == "roman")
                return IntToRoman(input, format.Equals("ROMAN"));
            return string.Format("{0:" + format + "}", input);
        }

        public static string IntToRoman(object input, bool uppercase)
        {
            if (input == null)
                return null;

            int num = int.Parse(input.ToString());
            // storing roman values of digits from 0-9  
            // when placed at different places 
            string[] m = { "", "M", "MM", "MMM" };
            string[] c = {"", "C", "CC", "CCC", "CD", "D",
                            "DC", "DCC", "DCCC", "CM"};
            string[] x = {"", "X", "XX", "XXX", "XL", "L",
                            "LX", "LXX", "LXXX", "XC"};
            string[] i = {"", "I", "II", "III", "IV", "V",
                            "VI", "VII", "VIII", "IX"};

            // Converting to roman 
            string thousands = m[num / 1000];
            string hundereds = c[(num % 1000) / 100];
            string tens = x[(num % 100) / 10];
            string ones = i[num % 10];

            string ans = thousands + hundereds + tens + ones;

            return ( uppercase ? ans : ans.ToLower());
        }

    }
}
