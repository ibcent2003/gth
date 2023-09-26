using Project.DAL;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using sw = GNSW.DAL;
using System.Data;

namespace Project.Models
{
    public class EServicesViewModel
    {

        public string CategoryName
        {
            get;
            set;
        }

        [Required(ErrorMessage = "Please select a country")]
        public int CountryId
        {
            get;
            set;
        }

        public List<IntegerSelectListItem> CountryList
        {
            get;
            set;
        }

        public string CountryName
        {
            get;
            set;
        }

        public List<DocumentCategory> DocumentCategoryList
        {
            get;
            set;
        }

        public DocumentInfo Documentinfo
        {
            get;
            set;
        }

        public List<DocumentInfo> DocumentInfoList
        {
            get;
            set;
        }

        public string DownloadPath
        {
            get;
            set;
        }

        public List<PhysicalExamination> Examination
        {
            get;
            set;
        }

        public List<ExporterProduct> ExporterProductList
        {
            get;
            set;
        }

        public List<FreightRate> FreightRateList
        {
            get;
            set;
        }

        public List<FreightValue> FreightValueList
        {
            get;
            set;
        }

        public bool hasSearch
        {
            get;
            set;
        }

        public ImportExportProcedure importexportProceedure
        {
            get;
            set;
        }

        public MobileTrackingForm MobileTrackingform
        {
            get;
            set;
        }

        public sw.MessageTemplate msg
        {
            get;
            set;
        }

        public Organization organisation
        {
            get;
            set;
        }

        public List<OrganizationFee> organisationFee
        {
            get;
            set;
        }

        public OrganizationFee OrgFees
        {
            get;
            set;
        }

        public List<int> orgId
        {
            get;
            set;
        }

        public List<OverageRates> OverageRatesList
        {
            get;
            set;
        }

        public Product product
        {
            get;
            set;
        }

        public List<Product> ProductList
        {
            get;
            set;
        }

        public string ProductName
        {
            get;
            set;
        }

        public List<ProductType> ProductTypeList
        {
            get;
            set;
        }

        public int TraderRegistrationId
        {
            get;
            set;
        }

        public List<SuccessDetail> UCRList
        {
            get;
            set;
        }

        public int VehicleCategoryId
        {
            get;
            set;
        }

        public List<IntegerSelectListItem> VehicleTypeList
        {
            get;
            set;
        }

        public string VehicleTypeName
        {
            get;
            set;
        }

    }
    public class MobileTrackingForm
    {
        [Display(Name = "Tin")]
        [Required(ErrorMessage = "Please enter your Tin")]
        public string Tin
        {
            get;
            set;
        }

        [Display(Name = "UCR No")]
        [Required(ErrorMessage = "Please enter your UCR No")]
        public string UCRNo
        {
            get;
            set;
        }
    }

    public class SuccessDetail
    {
        public string code { get; set; }

        public DateTime Data
        {
            get;
            set;
        }

        public int key
        {
            get;
            set;
        }

        public string message
        {
            get;
            set;
        }


    }

    public class RootObject
    {
        public int code
        {
            get;
            set;
        }

        public string message
        {
            get;
            set;
        }

        public ResponseDetail responseDetail
        {
            get;
            set;
        }

        public int status
        {
            get;
            set;
        }

       
    }

    public class ResponseDetail
    {
        public List<SuccessDetail> successDetail
        {
            get;
            set;
        }

    }

}