using PAARS.DAL;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Web.Mvc;

namespace Project.Models
{
    public class Sample
    {
        public string ChassisNo{get;set;}

        public ContactUsForm contactusform{get;set;}

        public int CountVehicle{get;set;}

        public SelectList CurrencyList{get;set;}

        public DutyFeedbackForm Dutyfeedbacform{get;set;}

        public decimal EngineCapacity{get;set;}

        public GeneralGoodForm generalform{get;set;}

        public bool hasBechMakeValue{get;set;}

        public decimal HDV{get;set;}

        public int ID {get;set;}

        public string MakeName {get;set;}

        public IList<SelectListItem> MakeNames{get;set;}

        public string ManufactureYear{get;set;}

        public string ModelName{get;set;}

        public IList<SelectListItem> ModelNames{get;set;}

        public GeneralGoodsResult resultform{get; set;}

        public List<string> SearchList{get;set;}

        public UsedVehicleDutyResultForm UsedVehicleDutyResultForm{get;set;}

        public ValuationTPD ValuationTPD{get; set;}

        public List<ValuationTPD> VehicleList{get;set;}

        public List<IntegerSelectListItem> VehicleModel{get;set;}

        public VehicleTypeForm VehicleTypeform{get;set;}

        public List<string> VehicleTypeList{get;set;}

        public List<IntegerSelectListItem> VehicleTypeLists { get; set; }

        public List<IntegerSelectListItem> VehicleTypeMake{get;set;}

        public string VehicleTypeName{get;set;}

        public Sample()
        {
        }
    }
}