using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Project.DAL;
using System.ComponentModel.DataAnnotations;

namespace Project.Areas.Setup.Models
{
    public class UsefullinkViewModel
    {
        public IList<UsefulLink> Rows { get; set; }
        public UsefulLinkForm UsefulLinkform { get; set; }
    }

    public class UsefulLinkForm
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Please enter the Name")]
        [Display(Name = "Name")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Please enter the Link")]
        [Display(Name = "Link")]
        public string Link { get; set; }

    }
}