using Project.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Project.Models
{
    public class RecentNewsViewModel
    {
        public List<News> NewsList { get; set; }
        public string PicturePath { get; set; }
        public List<string> newsCategory { get; set; }
        public List<DateTime> newslist { get; set; }
    }
}