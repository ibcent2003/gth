using GNSW.DAL;
using Project.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Project.Areas.Setup.Models
{
    public class UsedVehicleManagementViewModel
    {
        public List<IntegerSelectListItem> MakeList { get; set; }
        public IList<VehicleReference> Rows { get; set; }
        public VehicleReference VehicleReference { get; set; }
        public IList<VehicleModelReference> VModelList { get; set; }
        public VehicleManagementForm VehicleManagementform { get; set; }
        public ModelForm modelform { get; set; }

        public int VehicleId { get; set; }
    }

    public class VehicleManagementForm
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Please enter the Vehicle Make")]
        [Display(Name = "Vehicle Make")]
        public string Name { get; set; }
    }


    public class ModelForm
    {
        public int Id { get; set; }


        [Required(ErrorMessage = "Please enter the Model Name")]
        [Display(Name = "Model Name")]
        public string ModelName { get; set; }

        [Required(ErrorMessage = "Please Select the Make Type")]
        [Display(Name = "Make Type")]
        public int MakeId { get; set; }       

    }
}