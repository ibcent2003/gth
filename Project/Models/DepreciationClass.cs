using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Project.Models
{
    public class DepreciationClass
    {
        public decimal CalDepreciation(int year, decimal hdv, string half)
        {
            var curYear = DateTime.Now.Year;
            decimal resp = 0.00m;
            if (curYear + 1 == year)
            {

            }
            else if (curYear == year)
            {
                //if(year)
                switch (half)
                {
                    case "First":
                        resp = hdv * 0.00m;
                        break;
                    case "Second":
                        resp = hdv * 0.15m;
                        break;
                    default:
                        break;
                }
            }
            else if (curYear - 1 == year)
            {
                switch (half)
                {
                    case "First":
                        resp = hdv * 0.15m;
                        break;
                    case "Second":
                        resp = hdv * 0.30m;
                        break;
                    default:
                        break;
                }
            }
            else if (curYear - 2 == year)
            {
                switch (half)
                {
                    case "First":
                        resp = hdv * 0.30m;
                        break;
                    case "Second":
                        resp = hdv * 0.40m;
                        break;
                    default:
                        break;
                }
            }
            else if (curYear - 3 == year)
            {

                resp = hdv * 0.40m;

            }
            else if (curYear - 4 == year)
            {

                resp = hdv * 0.40m;

            }
            else if (curYear - 5 >= year)
            {
                resp = hdv * 0.50m;
            }
            return resp;
            //return null;
        }
    }
}