using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Project.Models
{
    public class TinVerificationObj
    {
        public string TIN { get; set; }
        public string CompanyName { get; set; }
        public string Address { get; set; }
        //public string OtherBusinessName { get; set; }
        public IList<string> OtherBusinessName { get; set; }
    }
}