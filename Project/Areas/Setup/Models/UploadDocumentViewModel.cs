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
    public class UploadDocumentViewModel
    {
        public IList<DocumentInfo> Rows { get; set; }

        public string path { get; set; }
        public List<IntegerSelectListItem> DocumentCategoryList { get; set; }
        public DocumentForm documentform { get; set; }
    }

    public class DocumentForm
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Please enter the Name")]
        [Display(Name = "Name")]
        public string Name { get; set; }


        [Required(ErrorMessage = "Please upload document")]
        public HttpPostedFileBase document { get; set; }

        [Required(ErrorMessage = "Select a Category Name")]
        [Display(Name = "Name")]
        public int DocumentCategoryId { get; set; }

        [Display(Name = "Extental Link")]
        public string ExtentalLink { get; set; }

        [Display(Name = "WebSite Link")]
        public string WebSiteLink { get; set; }
        }
    
}