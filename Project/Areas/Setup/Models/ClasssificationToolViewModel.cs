using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using Classification.DAL;
using Newtonsoft.Json.Linq;

namespace Project.Areas.Setup.Models
{
    public class ClasssificationToolViewModel
    {
        public ClassificationForm classificationForm
        {
            get;
            set;
        }

        public IList<HSCodes> ClassificationList
        {
            get;
            set;
        }

        public bool HasSearch
        {
            get;
            set;
        }

        public JArray JSonFormat
        {
            get;
            set;
        }

        public class ClassificationForm
        {
            [Display(Name = "Description")]
            [Required(ErrorMessage = "Please enter the Description")]
            public string Description
            {
                get;
                set;
            }
        }
    }
   
}