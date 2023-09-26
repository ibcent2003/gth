using Project.Areas.SecurityGuard.ViewModels;
using Project.DAL;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using sw = GNSW.DAL;

namespace Project.Models
{
    public class StakeholderRegistrationModel
    {
        public List<IntegerSelectListItem> TaxOfficeList { get; set; }
        public List<IntegerSelectListItem> region { get; set; }
        public List<IntegerSelectListItem> district { get; set; }
        public CompanyRegistrationFrom CompanyRegFrom { get; set; }

        public TempUser tempUser { get; set; }
        public bool TempUseradded { get; set; }
        public List<IntegerSelectListItem>MembershipList { get; set; }
        

        public List<IntegerSelectListItem> AssocaitionOne { get; set; }

        public List<IntegerSelectListItem> AssocaitionTwo { get; set; }

        public List<IntegerSelectListItem> AssocaitionThree { get; set; }
        public string MembershipNumber { get; set; }

       

        public int associationId { get; set; }

        public CompanyAddressForm companyAddressform { get; set; }

        public CompanyRepresentativeForm ContactPersonform { get; set; }

        public sw.Organisation organisation { get; set; }
        public sw.AddressBook address { get; set; }
        public string Id { get; set; }

        public List<sw.Categories> CategoryList { get; set; }
        public List<int> CategoryUsed { get; set; }
        public List<string> CategorySelected { get; set; }
        public List<string> CategorySelectedText { get; set; }
        public List<int> CompanyCategorySelected { get; set; }
        public string txt_10 { get; set; }
        public IEnumerable<HttpPostedFileBase> UploadedDocuments { get; set; }
        public bool DocumentsUploaded { get; set; }

        public List<sw.DocumentType> RequiredDocs { get; set; }
        public List<sw.DocumentInfo> UploadedDoc { get; set; }
        public bool AllDocumentUploaded { get; set; }
        public string DownloadPath { get; set; }
        public string newDocumentPath { get; set; }

       // public List<Memberform> membershipForm { get; set; }

        public MembershipForm MembershipForm { get; set; }

        public List<sw.Association> RequiredAssociation { get; set; }
        public List<sw.AddressBook> CompanyAddress { get; set; }

        public List<sw.ContactInfo> ContactInfo { get; set; }

        public DocumentForm documentform { get; set; }

        public AccountInformationForm accountForm { get; set; }

        public RegisterViewModel regform { get; set; }
        public Dictionary<int, List<IntegerSelectListItem>> MembershipListModel { get; internal set; }

        public StakeholderRegistrationModel()
        {
         //   membershipForm = new List<Memberform>();
        }

    }

    public class Memberform
    {
        public string MembershipNumber { get; set; }
        public int AssociationId { get; set; }
    }

    public class CompanyRegistrationFrom
    {
        [Required(ErrorMessage = "Please enter your company name")]
        [Display(Name = "Company Name")]
        public string CompanyName { get; set; }


        [Required(ErrorMessage = "Please enter your company Registration Number")]
        [Display(Name = "Registration Number")]
        public string RegistrationNumber { get; set; }


        [Required(ErrorMessage = "Please enter your company TIN")]
        [Display(Name = "Company TIN")]
        public string TIN { get; set; }

       
        [Display(Name = "Old IDF Application TIN ")]
        public string OlTIN { get; set; }


        [Required(ErrorMessage = "Please enter your Tax Office")]
        [Display(Name = "District Tax Office")]
        public int TaxOfficeId { get; set; }



    }

    public class CompanyAddressForm
    {
        [Required(ErrorMessage = "Street Name is required")]
        [Display(Name = "Physical Business Address")]
        public string Street { get; set; }
        //


        [Required(ErrorMessage = "Please select your Region")]
        [Display(Name = "Region")]
        public int regionId { get; set; }


        [Required(ErrorMessage = "Please select your District")]
        [Display(Name = "District")]
        public int districtId { get; set; }


        [Required(ErrorMessage = "Company Phone Number is required")]
        [Display(Name = "Phone Number")]
        public string PhoneNumber { get; set; }

    
        [Required(ErrorMessage = "Company Email Address is required")]
        [Display(Name = "Email Address")]
        public string EmailAddress { get; set; }




    }

    public class CompanyRepresentativeForm
    {
        [Required(ErrorMessage = "First Name is required")]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }


        [Required(ErrorMessage = "Last Name is required")]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }


        [Required(ErrorMessage = "Contact Person Phone Number is required")]
        [Display(Name = "Phone Number")]
        public string PhoneNumber { get; set; }


        [Required(ErrorMessage = "Contact Person Email Address is required")]
        [Display(Name = "Email Address")]
        public string EmailAddress { get; set; }
    }


    public class DocumentForm
    {
        //[Required]
        //[Display(Name = "Date Issued")]
        //public DateTime[] IssuedDate { get; set; }

        //[Display(Name = "Expiry Date")]
        //public DateTime?[] ExpiryDate { get; set; }

        //[Required(ErrorMessage = "Please upload all documents")]
       
    }

    public class MembershipForm
    {
        [Required(ErrorMessage = "Please select your Assocaition")]
        [Display(Name = "Assocaition")]
        public int[] MembershipId { get; set; }


        [Required(ErrorMessage = "Please provide your Membership Number ")]
        [Display(Name = "Membership Number")]
        public string[] MembershipNumber { get; set; }
    }

    public class AccountInformationForm
    {
        [Required(ErrorMessage = "The Username is required")]
        [Display(Name ="Username")]
        public string Username { get; set; }


        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [Required]

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [System.Web.Mvc.Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }


        public bool Approve { get; set; }


    }

}