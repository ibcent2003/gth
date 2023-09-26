using Project.DAL;
using Project.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;


namespace Project.Areas.Setup.Models
{
    public class FAQViewModel
    {
        public IList<FAQ> Rows { get; set; }
        public FAQForm faqform { get; set; }
    }

    public class FAQForm
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Please enter the Question")]
        [Display(Name = "Question")]
        public string Question { get; set; }

        [Required(ErrorMessage = "Please enter the Answer")]
        [Display(Name = "Answer")]
        public string Answer { get; set; }

    }
}