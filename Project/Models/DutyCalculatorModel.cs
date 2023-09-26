using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using sw = GNSW.DAL;
using pass = PAARS.DAL;
using System.ComponentModel.DataAnnotations.Schema;

namespace Project.Models
{
    public class DutyCalculatorModel
    {
        public DutyCalculatorModel()
        {

        }
        public ContactUsForm contactusform { get; set; }
        public IList<SelectListItem> MakeNames { get; set; }
        public IList<SelectListItem> ModelNames { get; set; }

        public DutyFeedbackForm Dutyfeedbacform { get; set; }

        public SelectList CurrencyList { get; set; }
        public GeneralGoodForm generalform { get; set; }

        public GeneralGoodsResult resultform { get; set; }
        public List<IntegerSelectListItem> VehicleTypeLists { get; set; }
        public List<IntegerSelectListItem> VehicleTypeMake { get; set; }

        public List<IntegerSelectListItem> VehicleModel { get; set; }


        public List<String> VehicleTypeList { get; set; }

        public VehicleTypeForm VehicleTypeform { get; set; }

        public UsedVehicleDutyResultForm UsedVehicleDutyResultForm { get; set; }

        public string MakeName { get; set; }

        public string ChassisNo { get; set; }

        public string VehicleTypeName { get; set; }

        public string ModelName { get; set; }

        public string ManufactureYear { get; set; }

        public decimal EngineCapacity { get; set; }
        public bool hasBechMakeValue { get; set; }
        public decimal HDV { get; set; }
        public int ID { get; set; }
        public int CountVehicle { get; set; }

        public List<pass.ValuationTPD> VehicleList { get; set; }

        public pass.ValuationTPD ValuationTPD { get; set; }

        public List<String> SearchList { get; set; }

    }

    public class GeneralGoodForm
    {
        [Display(Name = "Currency")]
        [Required(ErrorMessage = "Please select your Currency")]
        public string CurrencyId{get;set;}

        [Display(Name = "Freight")]
        public string Freight{get;set;}

        [Display(Name = "HS Code")]
        [Required(ErrorMessage = "Please Provide your hscode")]
        public string HSCode{get;set;}

        [Display(Name = "Insurance")]
        [Required(ErrorMessage = "Please provide your Insurance ")]
        public string Insurance{get;set;}

        [Display(Name = "Product Value")]
        public string ProductValue{get;set;}
    }


    public class GeneralGoodsResult
    {
        public string AfricanUnion {get;set;}

        public string CasseteLevy{get;set;}

        public string CIF {get;set;}

        public string CustomsInspectionFee{get;set;}

        public string DutyPayable{get;set;}

        public string EcoLevy{get;set;}

        public string EnivronmentalExcise {get;set;}

        public string ExchangeRate {get;set;}

        public string Exim{get;set;}

        public string FOB{get;set;}

        public string GetFund{get;set;}

        public string ImportDuty{get;set;}

        public string ImportLevey{get; set;}

        public string ImportNHIL{get;set;}

        public string ImportVat{get;set;}

        public string IRSTaxDeposit{get;set;}

        public string NHILPayble{get;set;}

        public string ProcessingFee{get;set;}

        public string SIL{get;set;}

        public string TotalDuty{get;set;}

        public string TotalDutyPayable{get;set;}

        public string UnitOfQuality{get;set;}

        public string VatPayble{get;set;
        }



    }

    public class VehicleTypeForm
    {
      
        public int MakeId { get; set; }

        public int ModelId { get; set; }
        
        public string Year { get; set; }
       
    }

    public class UsedVehicleDutyResultForm
    {
        public string cif
        {
            get;
            set;
        }

        public string cifafterdedution
        {
            get;
            set;
        }

        public string depreciation
        {
            get;
            set;
        }

        public string depreciationafterdedution
        {
            get;
            set;
        }

        public string ecowas
        {
            get;
            set;
        }

        public string ecowasafterdedution
        {
            get;
            set;
        }

        public string edif
        {
            get;
            set;
        }

        public string edifafterdedution
        {
            get;
            set;
        }

        public string examfee
        {
            get;
            set;
        }

        public string examfeeafterdedution
        {
            get;
            set;
        }

        public decimal FOB
        {
            get;
            set;
        }

        public decimal FOBafterdedution
        {
            get;
            set;
        }

        public string freight
        {
            get;
            set;
        }

        public string freightafterdedution
        {
            get;
            set;
        }

        public string ghcedis
        {
            get;
            set;
        }

        public string ghcedisafterdedution
        {
            get;
            set;
        }

        public string Hscode
        {
            get;
            set;
        }

        public string Hscodeafterdedution
        {
            get;
            set;
        }

        public string Importduty
        {
            get;
            set;
        }

        public string Importdutyafterdedution
        {
            get;
            set;
        }

        public decimal insurance
        {
            get;
            set;
        }

        public decimal insuranceafterdedution
        {
            get;
            set;
        }

        public string interestCharge
        {
            get;
            set;
        }

        public string interestChargeafterdedution
        {
            get;
            set;
        }

        public string localCurrency
        {
            get;
            set;
        }

        public string localCurrencyafterdedution
        {
            get;
            set;
        }

        public string NetchargeNhil
        {
            get;
            set;
        }

        public string NetchargeNhilafterdedution
        {
            get;
            set;
        }

        public string NetchargeVat
        {
            get;
            set;
        }

        public string NetchargeVatafterdedution
        {
            get;
            set;
        }

        public string networkCharge
        {
            get;
            set;
        }

        public string networkChargeafterdedution
        {
            get;
            set;
        }

        public string nhil
        {
            get;
            set;
        }

        public string nhilafterdedution
        {
            get;
            set;
        }

        public string Overage
        {
            get;
            set;
        }

        public string Overageafterdedution
        {
            get;
            set;
        }

        public string ProcessingFee
        {
            get;
            set;
        }

        public string ProcessingFeeafterdedution
        {
            get;
            set;
        }

        public string rate
        {
            get;
            set;
        }

        public string rateafterdedution
        {
            get;
            set;
        }

        public string shppers
        {
            get;
            set;
        }

        public string shppersafterdedution
        {
            get;
            set;
        }

        public string specialLevy
        {
            get;
            set;
        }

        public string specialLevyafterdedutiony
        {
            get;
            set;
        }

        public string vat
        {
            get;
            set;
        }

        public string vatafterdedution
        {
            get;
            set;
        }

    }


    public class DutyFeedbackForm
    {
        [Required(ErrorMessage = "Please enter your Full Name")]
        [Display(Name = "Full Name")]
        public string FullName { get; set; }

        [Required(ErrorMessage = "Please enter your Email Address")]
        [Display(Name = "Email Address")]
        public string EmailAddress { get; set; }


        [Required(ErrorMessage = "Please enter your Message")]
        [Display(Name = "Message")]
        public string Message { get; set; }
    }

}