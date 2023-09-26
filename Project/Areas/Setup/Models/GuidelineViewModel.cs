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
    public class GuidelineViewModel
    {
        public string PicturePath { get; set; }
        public IList<Guideline> Rows { get; set; }
       public GuidelineForm guidelineform { get; set; }

        public string filename { get; set; }
    }

    public class GuidelineForm
    {

        public int Id { get; set; }

        [Required(ErrorMessage = "Please enter the Guideline Name")]
        [Display(Name = "News Headline")]
        public string Name { get; set; }


        [Required(ErrorMessage = "Please enter the Description")]
        [Display(Name = "Description")]
        public string Description { get; set; }


        [Required(ErrorMessage = "Please upload document for this guideline")]
        public HttpPostedFileBase document { get; set; }


    


    }
}