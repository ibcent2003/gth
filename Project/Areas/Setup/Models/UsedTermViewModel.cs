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
    public class UsedTermViewModel
    {
        public IList<CommonlyUsedTerms> Rows { get; set; }
        public CommonlyUsedTermsForm usedtermform { get; set; }

        public GNSWTerm gnswterm { get; set; }

        public GNSWPrivacy gnswprivacy { get; set; }
    }

    public class CommonlyUsedTermsForm
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Please enter the Name")]
        [Display(Name = "Name")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Please enter the Description")]
        [Display(Name = "Description")]
        public string Description { get; set; }

    }


    public class GNSWTerm
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Please enter the Contenet Type")]
        [Display(Name = "Contenet Type")]
        public string ContenetType { get; set; }

        [Required(ErrorMessage = "Please enter the Contenet Information")]
        [Display(Name = "Contenet Information")]
        public string ContenetInformation { get; set; }
    }

    public class GNSWPrivacy
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Please enter the Contenet Type")]
        [Display(Name = "Contenet Type")]
        public string ContenetType { get; set; }

        [Required(ErrorMessage = "Please enter the Contenet Information")]
        [Display(Name = "Contenet Information")]
        public string ContenetInformation { get; set; }
    }
}