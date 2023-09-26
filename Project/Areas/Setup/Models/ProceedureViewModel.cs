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
    public class ProceedureViewModel
    {
        public IList<ImportExportProcedure> Rows { get; set; }
        public ProceeduresForm proceeduresForm { get; set; }
        public List<IntegerSelectListItem> ProcedureTypeList { get; set; }
    }

    public class ProceeduresForm
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Please enter the Name")]
        [Display(Name = "Name")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Please Select the proceedure Type")]
        [Display(Name = "Procedure Type")]
        public int ProcedureTypeId { get; set; }

        [Required(ErrorMessage = "Please enter the Description")]
        [Display(Name = "Description")]
        public string Description { get; set; }

       

    }
}