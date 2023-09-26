using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using Project.DAL;
using Project.Models;

namespace Project.Areas.Setup.Models
{
    public class ProductTypeViewModel
    {
        public IList<ProductType> Rows { get; set; }

        public IList<Product> products { get; set; }
        public ProductTypeForm producttypeform { get; set; }

        public List<IntegerSelectListItem> ProductTypeList { get; set; }
        public ProductForm productform { get; set; }

        public ProductType ProductType { get; set; }

        public string PicturePath { get; set; }
    }

    public class ProductTypeForm
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Please enter the Name")]
        [Display(Name = "Name")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Please enter the Description")]
        [Display(Name = "Description")]
        public string Description { get; set; }

        public string picture { get; set; }


        [Required(ErrorMessage = "Please upload Photo")]
        public HttpPostedFileBase Photo { get; set; }

    }


    public class ProductForm
    {
        public int Id { get; set; }


        [Required(ErrorMessage = "Please enter the Name")]
        [Display(Name = "Name")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Please Select the Product Type")]
        [Display(Name = "Product Type")]
        public int ProductTypeId { get; set; }

        [Required(ErrorMessage = "Please enter the Description")]
        [Display(Name = "Description")]
        public string Description { get; set; }

    }
}