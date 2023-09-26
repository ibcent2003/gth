using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;

namespace Project.Models
{
    public class HsCodeSlipter
    {
        public string Encode(string hscode)
        {
            hscode = hscode.Replace(" ", "");
            hscode = hscode.Replace(".", "");
            Regex regex = new Regex(@"^[0-9]+$");
            hscode = hscode.PadRight(10, '0');
            if (!regex.IsMatch(hscode)) return null;
            var fvar = hscode.Substring(0, 4);
            var svar = hscode.Substring(4, 2);
            var tvar = hscode.Substring(6, 2);
            var lvar = hscode.Substring(8);
            hscode = fvar + "." + svar + "." + tvar + "." + lvar;
            return hscode;
        }
    }
}