using Project.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Project.Areas.Setup.Models
{
    public class FeedbackMessageViewModel
    {
        public IList<ContactUs> Rows { get; set; }
    }
}