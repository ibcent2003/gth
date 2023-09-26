using Project.DAL;
using Project.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
//using GNSW.DAL;
using sRole = System.Web.Security.Roles;
using System.Text;
using System.Net.Http;
using Newtonsoft.Json;
using sw = GNSW.DAL;
using System.Web.UI.WebControls;
using Project.UI.Models;
using System.Web.Security;
using SecurityGuard.Services;
using SecurityGuard.Interfaces;
using System.Transactions;

namespace Project.Controllers
{
    public class StakeholderRegistrationController : Controller
    {
        private string processCode = Properties.Settings.Default.UserRegistrationWorkFlow;       
        private PROEntities db = new PROEntities();
        private sw.GNSWEntities swdb = new sw.GNSWEntities();   
        private int workflowId = 0;
        protected List<DocumentType> supportingDocuments = new List<DocumentType>();   
        protected List<DocumentInfo> tempDoc = new List<DocumentInfo>();
        private ProcessUtility util = new ProcessUtility();
        private IMembershipService membershipService;
        private IAuthenticationService authenticationService;
        private IFormsAuthenticationService formsAuthenticationService;



        public StakeholderRegistrationController()
        {
            this.membershipService = new MembershipService(Membership.Provider);
            this.authenticationService = new AuthenticationService(membershipService, new FormsAuthenticationService());
           this.formsAuthenticationService = new FormsAuthenticationService();

            workflowId = swdb.ApprovalWorkFlow.Where(x => x.ProcessCode == processCode).Select(x => x.Id).FirstOrDefault();
        }
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult CompanyInformation()
        {
            try
            {
                StakeholderRegistrationModel model = new StakeholderRegistrationModel();
                model.TaxOfficeList = (from s in swdb.TaxOffice select new IntegerSelectListItem { Text = s.Name, Value = s.Id }).ToList();
                return View(model);
            }
            catch (Exception ex)
            {
                TempData["messageType"] = "alert-danger";
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                return RedirectToAction("Error404", "Home", new { area = "" });
            }
        }

        [HttpPost]
        public ActionResult CompanyInformation(StakeholderRegistrationModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    model.TaxOfficeList = (from s in swdb.TaxOffice select new IntegerSelectListItem { Text = s.Name, Value = s.Id }).ToList();                                   
                    if (!IsTinValid(model.CompanyRegFrom.TIN))
                    {
                        model.TaxOfficeList = (from s in swdb.TaxOffice select new IntegerSelectListItem { Text = s.Name, Value = s.Id }).ToList();
                        TempData["MessageType"] = "danger";
                        TempData["Message"] = "Verification Failed: Please check that you entered the correct Company TIN and Company Name.";
                        return View(model);
                    }
                    else
                    {

                        //check existing company information
                        //var CheckUserName = swdb.Users.Where(x => x.UserName == model.CompanyRegFrom.TIN).ToList();
                        //if(CheckUserName.Any())
                        //{
                        //    model.TaxOfficeList = (from s in swdb.TaxOffice select new IntegerSelectListItem { Text = s.Name, Value = s.Id }).ToList();
                        //    TempData["MessageType"] = "danger";
                        //    TempData["Message"] = "The TIN has been used by another company. Please try again later or contact the system administrator.";
                        //    return View(model);
                        //}
                        var GetApprovalReg = swdb.Organisation.Where(x => x.TINNumber == model.CompanyRegFrom.TIN).FirstOrDefault();
                        var GetTaxcode = swdb.TaxOffice.Where(x => x.Id == model.CompanyRegFrom.TaxOfficeId).FirstOrDefault();
                        if (GetApprovalReg != null)
                        {
                            var Approval = GetApprovalReg.ApprovalRegistration.FirstOrDefault();
                            if (Approval == null)
                            {

                                //There is something in org table. just do update and contd

                                GetApprovalReg.Name = model.CompanyRegFrom.CompanyName;
                                GetApprovalReg.TINNumber = model.CompanyRegFrom.TIN;
                                GetApprovalReg.RCNumber = model.CompanyRegFrom.RegistrationNumber;
                                GetApprovalReg.TINNumber2 = model.CompanyRegFrom.OlTIN;
                                GetApprovalReg.TaxOfficeCode = GetTaxcode.Name;
                                GetApprovalReg.ModifiedBy ="System";
                                GetApprovalReg.ModifiedDate = DateTime.Now;
                                swdb.SaveChanges();
                                TempData["MessageType"] = "success";
                                TempData["Message"] = "Company Information added succesfully.";
                                return RedirectToAction("CompanyAddress", "StakeholderRegistration", new { Id = model.CompanyRegFrom.TIN });
                            }
                            else
                            {
                                model.TaxOfficeList = (from s in swdb.TaxOffice select new IntegerSelectListItem { Text = s.Name, Value = s.Id }).ToList();
                                TempData["MessageType"] = "danger";
                                TempData["Message"] = "You have already submitted your registration. Please login to proceed or contact the systm administrator";
                                return View(model);
                            }
                        }
                       else
                        {
                            //Do insert into organisation table
                            sw.Organisation addnew = new sw.Organisation
                            {
                                Name = model.CompanyRegFrom.CompanyName,
                                TINNumber = model.CompanyRegFrom.TIN,
                                TINNumber2 = model.CompanyRegFrom.OlTIN,
                                RCNumber = model.CompanyRegFrom.RegistrationNumber,
                                TaxOfficeCode =GetTaxcode.Name,
                                OrganisationTypeId = 2,
                                ModifiedBy = "System",
                                ModifiedDate = DateTime.Now
                            };
                            swdb.Organisation.AddObject(addnew);
                            swdb.SaveChanges();


                            model.TaxOfficeList = (from s in swdb.TaxOffice select new IntegerSelectListItem { Text = s.Name, Value = s.Id }).ToList();
                            TempData["MessageType"] = "success";
                            TempData["Message"] = "Company Information added succesfully.";
                            return RedirectToAction("CompanyAddress", "StakeholderRegistration", new { Id = model.CompanyRegFrom.TIN });
                        }                                 
                    }

                  
                }

                model.TaxOfficeList = (from s in swdb.TaxOffice select new IntegerSelectListItem { Text = s.Name, Value = s.Id }).ToList();
                return View(model);
            }
            catch (Exception ex)
            {
                TempData["MessageType"] = "danger";
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                return RedirectToAction("Error404", "Home", new { area = "" });
            }
        }



        [HttpPost]
        public ActionResult GetDistrict(int regionId)
        {
            List<IntegerSelectListItem> ListDistrict = (from d in swdb.District where d.RegionId == regionId orderby d.Name select new IntegerSelectListItem { Text = d.Name, Value = d.Id }).ToList();
            return Json(ListDistrict);
        }

        public ActionResult CompanyAddress(string Id=null)
        {
            try
            {

                if (Id == null)
                {
                
                    Elmah.ErrorSignal.FromCurrentContext().Raise(new Exception("No TIN sent to the company address Page: Company Registration"));
                    TempData["MessageType"] = "danger";
                    TempData["Message"] = "There is a problem with you application. Please try again or contact the system administrator";
                    return RedirectToAction("CompanyInformation");
                }

                StakeholderRegistrationModel model = new StakeholderRegistrationModel();
                var OrgObj = swdb.Organisation.Where(x => x.TINNumber == Id).FirstOrDefault();
                if (OrgObj == null)
                {
                    Elmah.ErrorSignal.FromCurrentContext().Raise(new Exception("Organisation object is null. Company Address method Page"));
                    TempData["MessageType"] = "danger";
                    TempData["Message"] = "There is a problem with you application. Please try again or contact the system administrator";
                    return RedirectToAction("CompanyInformation");
                }

                var Approval = OrgObj.ApprovalRegistration.FirstOrDefault();

                if (Approval != null)
                {
                    TempData["MessageType"] = "danger";
                    TempData["Message"] = "You have already submitted your registration. Please login to proceed or contact the systm administrator";
                    return RedirectToAction("CompanyInformation");
                }
                model.organisation = OrgObj;

                var addressbook =  model.organisation.AddressBook.FirstOrDefault();
                var contactperson = model.organisation.ContactInfo.FirstOrDefault();
                if(addressbook != null && contactperson != null)
                {
                   
                    model.companyAddressform = new CompanyAddressForm();
                    model.region = (from s in swdb.Region select new IntegerSelectListItem { Text = s.Name, Value = s.Id }).ToList();
                    model.district = (from d in swdb.District select new IntegerSelectListItem { Value = d.Id, Text = d.Name }).ToList();
                    model.companyAddressform.Street = addressbook.Street;
                    model.companyAddressform.regionId = int.Parse(addressbook.RegionId.ToString());
                    model.companyAddressform.districtId = int.Parse(addressbook.DistrictId.ToString());
                    model.companyAddressform.PhoneNumber = addressbook.PhoneNumber;
                    model.companyAddressform.EmailAddress = addressbook.EmailAddress;

                    model.ContactPersonform = new CompanyRepresentativeForm();
                    model.ContactPersonform.FirstName = contactperson.FirstName;
                    model.ContactPersonform.LastName = contactperson.LastName;
                    model.ContactPersonform.EmailAddress = contactperson.EmailAddress;
                    model.ContactPersonform.PhoneNumber = contactperson.MobileNumber;
                    model.organisation.TINNumber = Id;
                    return View(model);

                }
                          
                model.region = (from s in swdb.Region select new IntegerSelectListItem { Text = s.Name, Value = s.Id }).ToList();              
                model.district = (from d in swdb.District select new IntegerSelectListItem { Value = d.Id, Text = d.Name }).ToList();
                //model.CompanyRegFrom.TIN = model.organisation.TINNumber;
                return View(model);
            }
            catch(Exception ex)
            {
                TempData["MessageType"] = "danger";
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                return RedirectToAction("CompanyInformation");
            }
        }

        [HttpPost]
        public ActionResult CompanyAddress(StakeholderRegistrationModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {                 
                    var OrgObj = swdb.Organisation.Where(x => x.TINNumber == model.organisation.TINNumber).FirstOrDefault();
                    var addressbook = OrgObj.AddressBook.FirstOrDefault();
                    var contactperson = OrgObj.ContactInfo.FirstOrDefault();
                    if(addressbook != null && contactperson != null)
                    {
                        //do update address book
                        addressbook.Street = model.companyAddressform.Street;
                        addressbook.DistrictId = model.companyAddressform.districtId;
                        addressbook.PhoneNumber = model.companyAddressform.PhoneNumber;
                        addressbook.EmailAddress = model.companyAddressform.EmailAddress;
                        addressbook.RegionId = model.companyAddressform.regionId;
                        addressbook.ModifedDate = DateTime.Now;
                        addressbook.ModifiedBy = model.organisation.TINNumber;


                        //do update contact info
                        contactperson.FirstName = model.ContactPersonform.FirstName;
                        contactperson.LastName = model.ContactPersonform.LastName;
                        contactperson.EmailAddress = model.ContactPersonform.EmailAddress;
                        contactperson.MobileNumber = model.ContactPersonform.PhoneNumber;
                        contactperson.ModifiedDate = DateTime.Now;
                        contactperson.ModifiedBy = model.organisation.TINNumber;
                        swdb.SaveChanges();

                        model.region = (from s in swdb.Region select new IntegerSelectListItem { Text = s.Name, Value = s.Id }).ToList();
                        model.district = (from d in swdb.District select new IntegerSelectListItem { Value = d.Id, Text = d.Name }).ToList();
                        TempData["MessageType"] = "success";
                        TempData["Message"] = "Company Address has been added successfully";
                        return RedirectToAction("TypeOfBusiness", "StakeholderRegistration", new { Id = model.organisation.TINNumber });
                    }

                    model.region = (from s in swdb.Region select new IntegerSelectListItem { Text = s.Name, Value = s.Id }).ToList();
                    model.district = (from d in swdb.District select new IntegerSelectListItem { Value = d.Id, Text = d.Name }).ToList();
                    //insert new address book
                    sw.AddressBook address = new sw.AddressBook
                    {
                        AddressTypeId =1,
                        Street = model.companyAddressform.Street,
                        DistrictId = model.companyAddressform.districtId,
                        PhoneNumber = model.companyAddressform.PhoneNumber,
                        EmailAddress = model.companyAddressform.EmailAddress,
                        RegionId  = model.companyAddressform.regionId,
                        ModifedDate = DateTime.Now,
                        ModifiedBy = model.organisation.TINNumber
                    };
                    swdb.AddressBook.AddObject(address);
                    OrgObj.AddressBook.Add(address);

                    // insert new contact info
                    sw.ContactInfo contact = new sw.ContactInfo
                    {
                        FirstName = model.ContactPersonform.FirstName,
                        LastName = model.ContactPersonform.LastName,
                        MobileNumber = model.ContactPersonform.PhoneNumber,
                        EmailAddress = model.ContactPersonform.EmailAddress,
                        ModifiedBy = model.organisation.TINNumber,
                        ModifiedDate = DateTime.Now,
                    };
                    swdb.ContactInfo.AddObject(contact);
                    OrgObj.ContactInfo.Add(contact);
                    swdb.SaveChanges();

                    TempData["MessageType"] = "success";
                    TempData["Message"] = "Company Address has been added successfully";
                    return RedirectToAction("TypeOfBusiness", "StakeholderRegistration", new { Id = model.organisation.TINNumber });
                }
                model.region = (from s in swdb.Region select new IntegerSelectListItem { Text = s.Name, Value = s.Id }).ToList();
                model.district = (from d in swdb.District select new IntegerSelectListItem { Value = d.Id, Text = d.Name }).ToList();
                return View(model);
            }
            catch(Exception ex)
            {
                TempData["messageType"] = "alert-danger";
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                return RedirectToAction("CompanyInformation");
            }
        }


        public ActionResult TypeOfBusiness(string Id=null)
        {
            try
            {
                if (Id == null)
                {
                    Elmah.ErrorSignal.FromCurrentContext().Raise(new Exception("No TIN sent to the company address Page: Company Registration"));
                    TempData["MessageType"] = "danger";
                    TempData["Message"] = "There is a problem with you application. Please try again or contact the system administrator";
                    return RedirectToAction("CompanyInformation");
                }
                StakeholderRegistrationModel model = new StakeholderRegistrationModel();
                var OrgObject = swdb.Organisation.Where(x => x.TINNumber == Id).FirstOrDefault();
                var Approval = OrgObject.ApprovalRegistration.FirstOrDefault();

                if (Approval != null)
                {
                    TempData["MessageType"] = "danger";
                    TempData["Message"] = "You have already submitted your registration. Please login to proceed or contact the systm administrator";
                    return RedirectToAction("CompanyInformation");
                }
                List<int> checkedCompanyCategory = OrgObject.Categories.Select(x => x.Id).ToList();
                model.CategoryList = (from b in swdb.Categories where (b.EndDate == null || DateTime.Now < b.EndDate) select b).ToList();
                model.CategoryUsed = OrgObject.Categories.Select(x => x.Id).ToList();
                model.CompanyCategorySelected = checkedCompanyCategory;
                model.Id = Id;
                return View(model);
            }
            catch(Exception ex)
            {
                TempData["MessageType"] = "danger";
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                TempData["Message"] = "There is a problem with you application. Please try again or contact the system administrator";
                return RedirectToAction("CompanyInformation");
            }
        }

        [HttpPost]
        public ActionResult TypeOfBusiness(StakeholderRegistrationModel model)
        {
            try
            {
                model.CategoryList = (from b in swdb.Categories where (b.EndDate == null || DateTime.Now < b.EndDate) select b).ToList();
                if (model.CategoryUsed == null)
                {
                    TempData["MessageType"] = "danger";
                    TempData["Message"] = "ERROR: You must select at least ONE area of business before you can proceed";
                   //  model.MembershipList = (from s in swdb.Association select new IntegerSelectListItem { Text = s.Name, Value = s.Id }).ToList();
                    model.CategoryList = (from b in swdb.Categories where (b.EndDate == null || DateTime.Now < b.EndDate) select b).ToList();
                    return View(model);
                }
                if(ModelState.IsValid)
                {
                    var OrgObj = swdb.Organisation.Where(x => x.TINNumber == model.Id).FirstOrDefault();
                    string categorytext = "";
                    model.CategorySelectedText = new List<string> { };
                    foreach (int categoryid in model.CategoryUsed)
                    {
                        categorytext = (from b in swdb.Categories where b.Id == categoryid select b.Category).FirstOrDefault();
                        model.CategorySelectedText.Add(categorytext);
                    }

                    var del = OrgObj.Categories.ToList();
                    if (del != null)
                    {
                        foreach (var cate in del)
                        {
                            OrgObj.Categories.Remove(cate);
                            swdb.SaveChanges();
                        }
                    }

                    var AssOrg = swdb.OrganisationAssociation.Where(x => x.OrganisationId == OrgObj.Id).ToList();
                    if (AssOrg != null)
                    {
                        foreach (var ass in AssOrg)
                        {
                            var GetAss = swdb.OrganisationAssociation.Where(x => x.OrganisationId == OrgObj.Id && x.AssociationId == ass.AssociationId).FirstOrDefault();
                            sw.OrganisationAssociation delass = new sw.OrganisationAssociation
                            {

                            };
                            swdb.OrganisationAssociation.DeleteObject(GetAss);
                            swdb.SaveChanges();
                        }
                    }



                    foreach(int b in model.CategoryUsed.ToList())
                    {
                        if(model.CategoryUsed != null)
                        {
                            OrgObj.Categories.Add(swdb.Categories.FirstOrDefault(d => d.Id == b));
                            swdb.SaveChanges();
                        }
                    }
                     var selectedCat =  OrgObj.Categories.ToList();
                    foreach (var c in selectedCat)
                    {

                        if (c.RequireMembership == true)
                        {
                            return RedirectToAction("MembershipType", "StakeholderRegistration", new { Id = model.Id });                    
                        }
                       
                    }
                   return RedirectToAction("Document", "StakeholderRegistration", new { Id = model.Id });

                    
                   // model.CategoryList = (from b in swdb.Categories where (b.EndDate == null || DateTime.Now < b.EndDate) select b).ToList();
                    //   model.MembershipList = (from s in swdb.Association select new IntegerSelectListItem { Text = s.Name, Value = s.Id }).ToList();
                  

                }
                TempData["MessageType"] = "danger";
                TempData["Message"] = "ERROR: You must select at least ONE area of business before you can proceed";
               // model.MembershipList = (from s in swdb.Association select new IntegerSelectListItem { Text = s.Name, Value = s.Id }).ToList();
                model.CategoryList = (from b in swdb.Categories where (b.EndDate == null || DateTime.Now < b.EndDate) select b).ToList();
                return View(model);
            }
            catch(Exception ex)
            {
                TempData["MessageType"] = "danger";
                TempData["Message"] = "There is a problem with you application. Please try again or contact the system administrator";
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                return RedirectToAction("CompanyInformation");

            }

        }

        public ActionResult MembershipType(string Id = null)
        {
            try
            {
                if (Id == null)
                {
                    Elmah.ErrorSignal.FromCurrentContext().Raise(new Exception("No TIN sent to the company address Page: Company Registration"));
                    TempData["MessageType"] = "danger";
                    TempData["Message"] = "There is a problem with you application. Please try again or contact the system administrator";
                    return RedirectToAction("CompanyInformation");
                }
                StakeholderRegistrationModel model = new StakeholderRegistrationModel();
                var OrgObject = swdb.Organisation.Where(x => x.TINNumber == Id).FirstOrDefault();
                var Approval = OrgObject.ApprovalRegistration.FirstOrDefault();

                if (Approval != null)
                {
                    TempData["MessageType"] = "danger";
                    TempData["Message"] = "You have already submitted your registration. Please login to proceed or contact the systm administrator";
                    return RedirectToAction("CompanyInformation");
                }
                List<int> checkedCompanyCategory = OrgObject.Categories.Select(x => x.Id).ToList();
                model.CategoryList = (from b in swdb.Categories where (b.EndDate == null || DateTime.Now < b.EndDate) select b).ToList();

                var Used = OrgObject.Categories.Select(x => x.Id).ToList();
               // model.CategoryUsed = 
                model.CompanyCategorySelected = checkedCompanyCategory;

                List<int> getCate = (from c in swdb.Categories where c.RequireMembership==true select c.Id).ToList();
                List<int> CategoryUsed = new List<int>();
                model.CategoryUsed = CategoryUsed;
                model.Id = Id;               
                foreach (var cateUsed in Used)
                {                  
                    CategoryUsed.Add(cateUsed);                 
                }
                if(model.CategoryUsed != null)
                {
                    model.MembershipListModel = new Dictionary<int,List<IntegerSelectListItem>>(model.CategoryUsed.Count);
                    foreach (var cat in model.CategoryUsed)
                    {                     
                            var listModel = (from s in swdb.Association where s.CategoryId == cat select new IntegerSelectListItem { Text = s.Name, Value = s.Id }).ToList();
                        
                            model.MembershipListModel.Add(cat, listModel);             
                   }
                }
                return View(model);
            }
            catch (Exception ex)
            {
                TempData["MessageType"] = "danger";
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                TempData["Message"] = "There is a problem with you application. Please try again or contact the system administrator";
                return RedirectToAction("CompanyInformation");
            }
        }

        [HttpPost]
        public ActionResult MembershipType(StakeholderRegistrationModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var OrgObj = swdb.Organisation.Where(x => x.TINNumber == model.Id).FirstOrDefault();
                    List<int> checkedCompanyCategory = OrgObj.Categories.Select(x => x.Id).ToList();
                    model.CategoryList = (from b in swdb.Categories where (b.EndDate == null || DateTime.Now < b.EndDate) select b).ToList();

                    var Used = OrgObj.Categories.Select(x => x.Id).ToList();
                    // model.CategoryUsed = 
                    model.CompanyCategorySelected = checkedCompanyCategory;

                    List<int> getCate = (from c in swdb.Categories where c.RequireMembership == true select c.Id).ToList();
                    List<int> CategoryUsed = new List<int>();
                    model.CategoryUsed = CategoryUsed;
                 //   model.Id = Id;
                    foreach (var cateUsed in Used)
                    {
                        CategoryUsed.Add(cateUsed);
                    }

                    if (OrgObj != null)
                    {
                    
                    
                        string getMembershipId = Request.Form["MembershipForm.MembershipId"].ToString();
                        var ids = getMembershipId.Split(',');
                        string getMemberNo = Request.Form["MembershipForm.MembershipNumber"].ToString();
                        var mNo = getMemberNo.Split(',');
                        if(mNo.Contains(""))
                        {
                            model.CategoryList = (from b in swdb.Categories where (b.EndDate == null || DateTime.Now < b.EndDate) select b).ToList();                          
                            model.CompanyCategorySelected = checkedCompanyCategory;                     
                            if (model.CategoryUsed != null)
                            {
                                model.MembershipListModel = new Dictionary<int, List<IntegerSelectListItem>>(model.CategoryUsed.Count);
                                foreach (var cat in model.CategoryUsed)
                                {
                                    var listModel = (from s in swdb.Association where s.CategoryId == cat select new IntegerSelectListItem { Text = s.Name, Value = s.Id }).ToList();

                                    model.MembershipListModel.Add(cat, listModel);
                                }
                            }

                            TempData["messageType"] = "danger";
                            TempData["message"] = "Please enter your membership Number";
                            return View(model);
                        }
                        for (var i = 0; i < ids.Length; i++)
                        {
                           // var memId = int.Parse(ids[i]);
                          //  var memNO = mNo[i];
                            sw.OrganisationAssociation addnew = new sw.OrganisationAssociation
                            {
                                OrganisationId = OrgObj.Id,
                                AssociationId = int.Parse(ids[i]),
                                MembershipNumber = mNo[i]
                            };
                            swdb.OrganisationAssociation.AddObject(addnew);
                            swdb.SaveChanges();

                        }
                        return RedirectToAction("Document", "StakeholderRegistration", new { Id = model.Id });




                    }

                }

                    return View(model);
            }
            catch(Exception ex)
            {
                TempData["messageType"] = "danger";
                TempData["message"] = "An Error occur. Please try again later or contact the system administrator";
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                return RedirectToAction("CompanyInformation");
            }
        }

        public ActionResult AreaOfBusiness(string Id=null)
        {
            try
            {
                if (Id == null)
                {
                    Elmah.ErrorSignal.FromCurrentContext().Raise(new Exception("No TIN sent to the company address Page: Company Registration"));
                    TempData["MessageType"] = "danger";
                    TempData["Message"] = "There is a problem with you application. Please try again or contact the system administrator";
                    return RedirectToAction("CompanyInformation");
                }
                StakeholderRegistrationModel model = new StakeholderRegistrationModel();
               var OrgObject = swdb.Organisation.Where(x => x.TINNumber == Id).FirstOrDefault();
                List<int> checkedCompanyCategory = OrgObject.Categories.Select(x => x.Id).ToList();
               //  model.MembershipList = (from s in swdb.Association select new IntegerSelectListItem { Text = s.Name, Value = s.Id }).ToList();

                model.CategoryList = (from b in swdb.Categories where (b.EndDate == null || DateTime.Now < b.EndDate) select b).ToList();               
                if(OrgObject.Categories.Any())
                {
                 model.CategoryUsed = OrgObject.Categories.Select(x => x.Id).ToList();
                }
                model.CompanyCategorySelected = checkedCompanyCategory;
                model.Id = Id;
              

                return View(model);

            }
            catch(Exception ex)
            {
                TempData["MessageType"] = "danger";
                TempData["Message"] = "There is a problem with you application. Please try again or contact the system administrator";
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                return RedirectToAction("CompanyInformation");
            }
        }


        [HttpPost]
        public ActionResult AreaOfBusiness(StakeholderRegistrationModel model)
        {
            // var p = Request.Params["txt_10"];

            try
            {             
                if(model.CategoryUsed ==null)
                {
                    TempData["MessageType"] = "danger";
                    TempData["Message"] = "ERROR: You must select at least ONE area of business before you can proceed";
                    model.MembershipList = (from s in swdb.Association select new IntegerSelectListItem { Text = s.Name, Value = s.Id }).ToList();
                    model.CategoryList = (from b in swdb.Categories where (b.EndDate == null || DateTime.Now < b.EndDate) select b).ToList();
                    return View(model);
                }

                if (ModelState.IsValid)
                {


                    model.CategoryList = (from b in swdb.Categories where (b.EndDate == null || DateTime.Now < b.EndDate) select b).ToList();
                    var OrgObj = swdb.Organisation.Where(x => x.TINNumber == model.Id).FirstOrDefault();
                    string categorytext = "";
                    model.CategorySelectedText = new List<string> { };
                    foreach(int categoryid in model.CategoryUsed)
                    {
                        categorytext = (from b in swdb.Categories where b.Id == categoryid select b.Category).FirstOrDefault();
                        model.CategorySelectedText.Add(categorytext);
                    }

                    var del = OrgObj.Categories.ToList();
                    if(del != null)
                    {
                        foreach (var cate in del)
                        {                        
                            OrgObj.Categories.Remove(cate);                            
                            swdb.SaveChanges();
                        }
                    }

                    var AssOrg = swdb.OrganisationAssociation.Where(x => x.OrganisationId == OrgObj.Id).ToList();
                    if(AssOrg != null)
                    {
                        foreach(var ass in AssOrg)
                        {
                            var GetAss = swdb.OrganisationAssociation.Where(x => x.OrganisationId == OrgObj.Id && x.AssociationId == ass.AssociationId).FirstOrDefault();
                            sw.OrganisationAssociation delass = new sw.OrganisationAssociation
                            {

                            };
                            swdb.OrganisationAssociation.DeleteObject(GetAss);
                            swdb.SaveChanges();
                        }
                    }

                    foreach(int b in model.CategoryUsed.ToList())
                    {
                        if(model.CategoryUsed != null)
                        {
                            var checkcate = swdb.Association.Where(x => x.CategoryId == b).ToList();
                            if(checkcate.Any())
                            {
                                foreach(var m in model.MembershipList)
                                {
                                    //insert into organisation category and organisation association
                                    var GetCate = swdb.Association.Where(x => x.CategoryId == b && x.Id == m.Value).FirstOrDefault();
                                    if(GetCate != null)
                                    {                                      
                                        //insert into organisation category 
                                        OrgObj.Categories.Add(swdb.Categories.FirstOrDefault(d => d.Id == b));
                                        //insert into Organisation Association
                                        sw.OrganisationAssociation addnew = new GNSW.DAL.OrganisationAssociation
                                        {
                                            OrganisationId = OrgObj.Id,
                                            AssociationId = m.Value,
                                            MembershipNumber = m.Text
                                        };
                                         swdb.OrganisationAssociation.AddObject(addnew);
                                        //Save all together
                                         swdb.SaveChanges();
                                    }
                                }
                            }
                            else
                            {
                                //just insert into organisation category only                               
                                OrgObj.Categories.Add(swdb.Categories.FirstOrDefault(d => d.Id == b));
                                swdb.SaveChanges();
                            }

                        }
                    }
                    model.MembershipList = (from s in swdb.Association select new IntegerSelectListItem { Text = s.Name, Value = s.Id }).ToList();
                    return RedirectToAction("Document", "StakeholderRegistration", new { Id = model.Id });
           
                }
                TempData["MessageType"] = "danger";
                TempData["Message"] = "ERROR: You must select at least ONE area of business before you can proceed";
                model.MembershipList = (from s in swdb.Association select new IntegerSelectListItem { Text = s.Name, Value = s.Id }).ToList();
                model.CategoryList = (from b in swdb.Categories where (b.EndDate == null || DateTime.Now < b.EndDate) select b).ToList();
                return View(model);
            }
            catch(Exception ex)
            {
                TempData["MessageType"] = "danger";
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                return RedirectToAction("CompanyInformation");
            }
        }


        [HttpPost]
        public ActionResult GetCategoryList(int categoryId)
        {
            try
            {
                List<IntegerSelectListItem> ListCategory = (from d in swdb.Association where d.CategoryId == categoryId orderby d.Name select new IntegerSelectListItem { Text = d.Name, Value = d.Id }).ToList();
                return Json(ListCategory);

            }
            catch(Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                return RedirectToAction("Error404", "Home", new { area = "" });
            }
           
        }
        public bool IsTinValid(string tin)
        {
            var valid = true;

            using (var client = new HttpClient())
            {

                var companyTinNum = tin;

                string baseuri = Properties.Settings.Default.MasterDataUrl;
                var tinNumber = "TaxIdentificationNumber?tin=" + companyTinNum.Trim();

                var targeturl = baseuri + tinNumber;

                Console.WriteLine("GET: + " + targeturl);

                var byteArray = Encoding.ASCII.GetBytes(Properties.Settings.Default.Username + ":" + Properties.Settings.Default.Key);
                client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));

                var responseTask = client.GetAsync(targeturl);

                responseTask.Wait();

                var result = responseTask.Result;
                if (result.IsSuccessStatusCode)
                {
                    var readTask = result.Content.ReadAsStringAsync().Result;
                    var datalist = JsonConvert.DeserializeObject<TinVerificationObj>(readTask);

                    if (string.IsNullOrEmpty(datalist.CompanyName)) valid = false;


                }


            }

            return valid;
        }


        public ActionResult Document(string Id)
        {
            try
            {

                if (Id == null)
                {
                    Elmah.ErrorSignal.FromCurrentContext().Raise(new Exception("No TIN sent to the company address Page: Company Registration"));
                    TempData["MessageType"] = "danger";
                    TempData["Message"] = "There is a problem with you application. Please try again or contact the system administrator";
                    return RedirectToAction("CompanyInformation");
                }
                StakeholderRegistrationModel model = new StakeholderRegistrationModel();
                var OrgObject = swdb.Organisation.Where(x => x.TINNumber == Id).FirstOrDefault();
                var Approval = OrgObject.ApprovalRegistration.FirstOrDefault();
                if (Approval != null)
                {
                    TempData["MessageType"] = "danger";
                    TempData["Message"] = "You have already submitted your registration. Please login to proceed or contact the systm administrator";
                    return RedirectToAction("CompanyInformation");
                }
                var GetWorkflow = swdb.ApprovalWorkFlow.Where(x => x.Id == 6).FirstOrDefault();
                model.UploadedDoc = OrgObject.DocumentInfo.Distinct().ToList();
                model.RequiredDocs = new List<sw.DocumentType>
                {
                };
                List<int> validdocument = new List<int> { };
                List<GNSW.DAL.DocumentInfo> docs = OrgObject.DocumentInfo.ToList();

                foreach (sw.DocumentInfo d in docs)
                {
                    //if (ExpChecker.checkExpiryCHAMS(d, d.DocumentType).Key == false)
                    //{
                        validdocument.Add(d.DocumentTypeId);
                 //   }
                }
                var doclist = swdb.ApprovalWorkFlowDocuments.Where(a => a.WorkFlowId == 6).Select(s => s.DocumentTypeId).ToList();

                var uploaded = swdb.ApprovalWorkFlowDocuments.Where(a => a.WorkFlowId == 6).Select(s => s.DocumentTypeId).ToList();

                List<GNSW.DAL.DocumentType> reqdocs = swdb.DocumentType.Where(d => d.Source == "User" && doclist.Contains(d.Id)).ToList();
                foreach (GNSW.DAL.DocumentType wfd in reqdocs)
                {
                    if (!validdocument.Contains(wfd.Id))
                    {
                        model.RequiredDocs.Add(wfd);
                    }
                }
                model.RequiredDocs =  swdb.DocumentType.Where(d => d.Source == "User" && doclist.Contains(d.Id)).ToList();
                if (model.RequiredDocs.Count() == model.UploadedDoc.Count())
                {
                    model.DocumentsUploaded = true;
                }
                if (model.UploadedDoc.Count > 0)
                {
                    var uploadeddocumentType = model.UploadedDoc.Select(d => d.DocumentTypeId).ToList();
                    model.RequiredDocs = model.RequiredDocs.Where(d => !uploadeddocumentType.Contains(d.Id)).ToList();
                }
                model.Id = Id;
                model.newDocumentPath = Properties.Settings.Default.NewDocumentPath;
                return View(model);
            }
            catch(Exception ex)
            {
                TempData["MessageType"] = "danger";
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                TempData["Message"] = "There is a problem with you application. Please try again or contact the system administrator";
                return RedirectToAction("CompanyInformation");
            }
        }

        [HttpPost]
        public ActionResult Document(StakeholderRegistrationModel model, IEnumerable<HttpPostedFileBase> documents)
        {
            try
            {

                var OrgObj = swdb.Organisation.Where(x => x.TINNumber == model.Id).FirstOrDefault();

                string url = Properties.Settings.Default.NewDocumentPath;
                System.IO.Directory.CreateDirectory(url);

                List<sw.DocumentInfo> uploadedfiles = new List<sw.DocumentInfo>();
                string EncKey = util.MD5Hash(DateTime.Now.Ticks.ToString());

                List<int> validatedocument = new  List<int> { };
                List<sw.DocumentInfo> docs = OrgObj.DocumentInfo.ToList();

                int max_upload_length = 5242880;

                var doclist = swdb.ApprovalWorkFlowDocuments.Where(a => a.WorkFlowId == 6).Select(s => s.DocumentTypeId).ToList();

                model.UploadedDoc = OrgObj.DocumentInfo.Union(OrgObj.DocumentInfo).Distinct().ToList();
                model.RequiredDocs = swdb.DocumentType.Where(d => d.Source == "User" && doclist.Contains(d.Id)).ToList();

                if(model.UploadedDoc.Count > 0)
                {
                    var uploadeddocumentTypes = model.UploadedDoc.Select(d => d.DocumentTypeId).ToList();
                    model.RequiredDocs = model.RequiredDocs.Where(d => !uploadeddocumentTypes.Contains(d.Id)).ToList();
                }
                model.UploadedDocuments = documents;


                /////////Validate Documents
                int InvalidDocs = 0;
                List<string> filetypes = new List<string> { };
                if (model.UploadedDocuments != null)
                {

                    int fcount = 0;
                    foreach (var file in model.UploadedDocuments)
                    {
                        ////////////////Validate Document Format

                        var doctype = swdb.DocumentType.FirstOrDefault(x => x.Id == 10);

                        filetypes = doctype.DocumentFormat.Select(x => x.Extension).ToList();

                        if (file != null)
                        {
                            if ((!filetypes.Contains(System.IO.Path.GetExtension(file.FileName))))
                            {
                                TempData["messageType"] = "danger";
                                TempData["message"] = "" + System.IO.Path.GetExtension(file.FileName) + " is not a valid file type for " + model.RequiredDocs[fcount].Name;
                                model.organisation = OrgObj;                                                           
                                model.newDocumentPath = Properties.Settings.Default.NewDocumentPath;
                                return View(model);
                            }
                            if (file.ContentLength > max_upload_length)
                            {
                                TempData["messageType"] = "danger";
                                TempData["message"] = "" + model.RequiredDocs[fcount].Name + " is larger than the 5MD upload limit";
                                model.organisation = OrgObj;                              
                                model.newDocumentPath = Properties.Settings.Default.NewDocumentPath;
                                return View(model);
                            }
                        }
                        else
                        {

                            TempData["messageType"] = "danger";
                            TempData["message"] = "No document uploaded for " + model.RequiredDocs[fcount].Name + "";
                            model.organisation = OrgObj;                           
                            model.newDocumentPath = Properties.Settings.Default.NewDocumentPath;
                            return View(model);

                        }
                        fcount++;
                    }
                    if (InvalidDocs > 0)
                    {
                        return View(model);
                    }
                }
                else
                {
                    TempData["MessageType"] = "danger";
                    TempData["message"] = "No document uploaded";
                    model.organisation = OrgObj;                 
                    model.newDocumentPath = Properties.Settings.Default.NewDocumentPath;
                    return View(model);

                }

                ///upload and insert into document 
                if (model.UploadedDocuments != null)
                {
                    int i = 0;
                    string filename;
                    foreach (var file in model.UploadedDocuments)
                    {
                        ///////UNTESTED
                        filename = EncKey + i + System.IO.Path.GetExtension(file.FileName);
                        file.SaveAs(url + filename);
                        uploadedfiles.Add(new GNSW.DAL.DocumentInfo { DocumentTypeId = model.RequiredDocs[i].Id, Name = filename, Path = filename, Size = file.ContentLength.ToString(), Extension = System.IO.Path.GetExtension(file.FileName), ModifiedDate = DateTime.Now, ModifiedBy = User.Identity.Name});
                        i++;
                    }
                }
                ////Insert Documents(DocumentInfo and CompanyDocumentInfo)
                foreach (GNSW.DAL.DocumentInfo d in uploadedfiles)
                {
                    swdb.DocumentInfo.AddObject(d);
                    OrgObj.DocumentInfo.Add(d);
                    swdb.SaveChanges();
                }
                model.newDocumentPath = Properties.Settings.Default.NewDocumentPath;
                TempData["message"] = "The documents has been added successully. Please click the Next button below to continue.";
                return RedirectToAction("Document", new { Id = model.Id });              

            }
            catch(Exception ex)
            {
                TempData["MessageType"] = "danger";
                TempData["message"] = "There is a problem with you application. Please try again or contact the system administrator";
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                return RedirectToAction("CompanyInformation");

            }
        }


       
        public ActionResult RemoveDocument(int DocId, string Id)
        {
            try
            {

                StakeholderRegistrationModel ViewModel = new StakeholderRegistrationModel();             
                ViewModel.newDocumentPath = Properties.Settings.Default.NewDocumentPath;
                var OrgObj = swdb.Organisation.Where(x => x.TINNumber == Id).FirstOrDefault();              
                ViewModel.UploadedDoc = OrgObj.DocumentInfo.Distinct().ToList();

                var GetDocumentInfo = (from d in swdb.DocumentInfo where d.Id == DocId select d).FirstOrDefault();

                if (GetDocumentInfo.Path != null)
                {
                    System.IO.FileInfo fi = new System.IO.FileInfo(ViewModel.newDocumentPath + GetDocumentInfo.Path);
                    fi.Delete();
                }
                GNSW.DAL.DocumentInfo del = new GNSW.DAL.DocumentInfo
                {
                };
                swdb.DocumentInfo.DeleteObject(GetDocumentInfo);
                OrgObj.DocumentInfo.Remove(GetDocumentInfo);
                if (OrgObj.DocumentInfo != null)
                {
                    OrgObj.DocumentInfo.Remove(GetDocumentInfo);
                    swdb.SaveChanges();
                }
                swdb.SaveChanges();
                TempData["message"] = "The documents has been deleted successully.";
                return RedirectToAction("Document", new { Id = Id });
            }
            catch (Exception ex)
            {
                TempData["messageType"] = "danger";
                TempData["message"] = "An Error occur. Please try again later or contact the system administrator";
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                return RedirectToAction("CompanyInformation");
            }

        }

        //[Authorize]
        public ActionResult DocumentsUploadedPath(string path)
        {
            try
            {

                var filepath = new Uri(path);
                if (System.IO.File.Exists(filepath.AbsolutePath))
                {
                    byte[] filedata = System.IO.File.ReadAllBytes(filepath.AbsolutePath);
                    string contentType = MimeMapping.GetMimeMapping(filepath.AbsolutePath);

                    System.Net.Mime.ContentDisposition cd = new System.Net.Mime.ContentDisposition
                    {
                        FileName = path,
                        Inline = true,
                    };

                    Response.AppendHeader("Content-Disposition", cd.ToString());

                    return File(filedata, contentType);
                }
                else
                {
                    return null;
                   
                }
            }
            catch (Exception ex)
            {

                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                return null;
            }
        }


        public ActionResult ChangePassword(string Id)
        {
            try
            {
                if (Id == null)
                {
                    Elmah.ErrorSignal.FromCurrentContext().Raise(new Exception("No TIN sent to the change password Page: Company Registration"));
                    TempData["MessageType"] = "danger";
                    TempData["Message"] = "There is a problem with you application. Please try again or contact the system administrator";
                    return RedirectToAction("CompanyInformation");
                }
                StakeholderRegistrationModel ViewModel = new StakeholderRegistrationModel();
                var OrgObject = swdb.Organisation.Where(x => x.TINNumber == Id).FirstOrDefault();
                var Approval = OrgObject.ApprovalRegistration.FirstOrDefault();

                if (Approval != null)
                {
                    TempData["MessageType"] = "danger";
                    TempData["Message"] = "You have already submitted your registration. Please login to proceed or contact the systm administrator";
                    return RedirectToAction("CompanyInformation");
                }
                if (OrgObject != null)
                {
                    var GetTempUser = db.TempUser.Where(x => x.Tin == Id).FirstOrDefault();
                    TempUser del = new TempUser
                    {

                    };
                    db.TempUser.DeleteObject(GetTempUser);
                    db.SaveChanges();
                    return RedirectToAction("AccountInformation", new { Id = Id });
                }
                return View(ViewModel);

            }
            catch(Exception ex)
            {
                TempData["messageType"] = "danger";
                TempData["message"] = "An Error occur. Please try again later or contact the system administrator";
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                return RedirectToAction("CompanyInformation");
            }
        }
        private static string ErrorCodeToString(MembershipCreateStatus createStatus)
        {           
            switch (createStatus)
            {
                case MembershipCreateStatus.DuplicateUserName:
                    return "User name already exists. Please enter a different user name.";
                

                case MembershipCreateStatus.InvalidPassword:
                    return "The password provided is invalid. Please enter a valid password value.";
                
                case MembershipCreateStatus.InvalidUserName:
                    return "The user name provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.ProviderError:
                    return "The authentication provider returned an error. Please verify your entry and try again. If the problem persists, please contact your system administrator.";

                case MembershipCreateStatus.UserRejected:
                    return "The user creation request has been canceled. Please verify your entry and try again. If the problem persists, please contact your system administrator.";

                default:
                    return "An unknown error occurred. Please verify your entry and try again. If the problem persists, please contact your system administrator.";
            }
        }
        public ActionResult AccountInformation(string Id=null)
        {
            try
            {
                if (Id == null)
                {

                    Elmah.ErrorSignal.FromCurrentContext().Raise(new Exception("No TIN sent to the company address Page: Company Registration"));
                    TempData["MessageType"] = "danger";
                    TempData["Message"] = "There is a problem with you application. Please try again or contact the system administrator";
                    return RedirectToAction("CompanyInformation");
                }
                StakeholderRegistrationModel model = new StakeholderRegistrationModel();
               var OrgObject = swdb.Organisation.Where(x => x.TINNumber == Id).FirstOrDefault();
                var Approval = OrgObject.ApprovalRegistration.FirstOrDefault();

                if (Approval != null)
                {
                    TempData["MessageType"] = "danger";
                    TempData["Message"] = "You have already submitted your registration. Please login to proceed or contact the systm administrator";
                    return RedirectToAction("CompanyInformation");
                }
                model.organisation = OrgObject;
                var checkTempUser = db.TempUser.Where(x => x.Tin == Id).FirstOrDefault();
                if(checkTempUser != null)
                {
                    model.tempUser = db.TempUser.Where(x => x.Tin == Id).FirstOrDefault();
                    model.Id = Id;
                    model.TempUseradded = true;                 
                    return View(model);
                }
                else
                {
                    model.accountForm = new AccountInformationForm();

                    model.Id = Id;
                    model.accountForm.Username = Id;
                    model.TempUseradded = false;
                    return View(model);

                }
               
            }
            catch(Exception ex)
            {
                TempData["messageType"] = "danger";
                TempData["message"] = "An Error occur. Please try again later or contact the system administrator";
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                return RedirectToAction("CompanyInformation");
            }
        }

        #region Register Methods


        [HttpPost]
        [ValidateAntiForgeryToken()]
        public ActionResult AccountInformation(StakeholderRegistrationModel model)
        {
            if (ModelState.IsValid)
            {
                var OrgObj = swdb.Organisation.Where(x => x.TINNumber == model.Id).FirstOrDefault();               
                if(model.accountForm.Password != model.accountForm.ConfirmPassword)
                {
                    TempData["messageType"] = "danger";
                    TempData["message"] = "The two password does not match. Please try again";
                    return View(model);
                }
                TempUser addnew = new TempUser
                {
                    Tin = OrgObj.TINNumber,
                    Password = model.accountForm.Password
                };
                db.TempUser.AddObject(addnew);
                db.SaveChanges();
                TempData["message"] = "The Account Information has been added successully.";
                return RedirectToAction("Review", new { Id = addnew.Tin });
            }

            return View(model);
        }

        public ActionResult Review(string Id=null)
        {
           try
            {
                if (Id == null)
                {

                    Elmah.ErrorSignal.FromCurrentContext().Raise(new Exception("No TIN sent to the company address Page: Company Registration"));
                    TempData["MessageType"] = "danger";
                    TempData["Message"] = "There is a problem with you application. Please try again or contact the system administrator";
                    return RedirectToAction("CompanyInformation");
                }
                StakeholderRegistrationModel model = new StakeholderRegistrationModel();
               var OrgObj = swdb.Organisation.Where(x => x.TINNumber == Id).FirstOrDefault();
                if(OrgObj==null)
                {
                    Elmah.ErrorSignal.FromCurrentContext().Raise(new Exception("Organisation obj is null. Review Page : line 1194"));
                    TempData["MessageType"] = "danger";
                    TempData["Message"] = "There is a problem with you application. Please try again or contact the system administrator";
                    return RedirectToAction("CompanyInformation");
                }              
                var Approval = OrgObj.ApprovalRegistration.FirstOrDefault();

                if(Approval != null)
                {
                    TempData["MessageType"] = "danger";
                    TempData["Message"] = "You have already submitted your registration. Please login to proceed or contact the systm administrator";
                    return RedirectToAction("CompanyInformation");
                }
                model.organisation = OrgObj;
                model.CompanyAddress = OrgObj.AddressBook.ToList();
                model.ContactInfo = OrgObj.ContactInfo.ToList();
                model.UploadedDoc = OrgObj.DocumentInfo.Distinct().ToList();
                model.Id =Id;
                var gthTemuser = db.TempUser.Where(x => x.Tin == Id).FirstOrDefault();
                if(gthTemuser != null)
                {
                    model.tempUser = gthTemuser;
                }
                model.newDocumentPath = Properties.Settings.Default.NewDocumentPath;
                return View(model);
            }
            catch(Exception ex)
            {
                TempData["messageType"] = "danger";
                TempData["message"] = "An Error occur. Please try again later or contact the system administrator";
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                return RedirectToAction("CompanyInformation");
            }
        }

        public ActionResult SubmitRegistration(string Id=null)
        {
            try
            {
                if (Id == null)
                {
                    Elmah.ErrorSignal.FromCurrentContext().Raise(new Exception("No TIN sent to the company address Page: Company Registration"));
                    TempData["MessageType"] = "danger";
                    TempData["Message"] = "There is a problem with you application. Please try again or contact the system administrator";
                    return RedirectToAction("CompanyInformation");
                }
                StakeholderRegistrationModel model = new StakeholderRegistrationModel();

                var now = DateTime.Now;
                var ownedBy = GetApprovalProcessOwnedBy();

                var OrgObj = swdb.Organisation.Where(x => x.TINNumber == Id).FirstOrDefault();
                if(OrgObj ==null)
                {
                    //throw expection
                    #region show error
                    Elmah.ErrorSignal.FromCurrentContext().Raise(new Exception("Cannot find Organisation"));
                    TempData["MessageType"] = "danger";
                    TempData["Message"] = "There is a problem with you application. Please try again or contact the system administrator";
                    return RedirectToAction("CompanyInformation");
                    #endregion
                }
                var tempUser = db.TempUser.Where(x => x.Tin == Id).FirstOrDefault();
                if(tempUser==null)
                {
                    //throw expection
                    #region show error
                    Elmah.ErrorSignal.FromCurrentContext().Raise(new Exception("Temp User table in gth db is null"));
                    TempData["MessageType"] = "danger";
                    TempData["Message"] = "There is a problem with you application. Please try again or contact the system administrator";
                    return RedirectToAction("CompanyInformation");
                    #endregion
                }
               
                var CategoryUsed = OrgObj.Categories.ToList();
                if(CategoryUsed == null)
                {
                    //throw expection
                    #region show error
                    Elmah.ErrorSignal.FromCurrentContext().Raise(new Exception("No Category Used is been selected"));
                    TempData["MessageType"] = "danger";
                    TempData["Message"] = "There is a problem with you application. Please try again or contact the system administrator";
                    return RedirectToAction("CompanyInformation");
                    #endregion
                }

                model.organisation = OrgObj;
                // Attempt to register the user
                MembershipCreateStatus createStatus;
                var Contact = OrgObj.ContactInfo.FirstOrDefault();
                if(Contact==null)
                {
                    //throw expection
                    #region show error
                    Elmah.ErrorSignal.FromCurrentContext().Raise(new Exception("Cannot find Organisation contact during final submission. The Object OrgObj.ContactInfo.FirstOrDefault()"));
                    TempData["MessageType"] = "danger";
                    TempData["Message"] = "There is a problem with you application. Please try again or contact the system administrator";
                    return RedirectToAction("CompanyInformation");
                    #endregion
                }
                int intPendingStatus = Properties.Settings.Default.UserRegistrationProcessPendingStatus;
                var status = swdb.RegStatus.Where(x => x.Id == intPendingStatus).FirstOrDefault();
                if(status==null)
                {
                    //throw expection                   
                    #region show error
                    Elmah.ErrorSignal.FromCurrentContext().Raise(new Exception("Cannot find status of "+ intPendingStatus + " in reg status table"));
                    TempData["MessageType"] = "danger";
                    TempData["Message"] = "There is a problem with you application. Please try again or contact the system administrator";
                    return RedirectToAction("CompanyInformation");
                    #endregion
                }
                var GetRoleAdmin = swdb.Roles.Where(x => x.RoleName == "Organisation Admin").FirstOrDefault();
                if(GetRoleAdmin == null)
                {
                    //throw expection                   
                    #region show error
                    Elmah.ErrorSignal.FromCurrentContext().Raise(new Exception("Cannot find role name " + GetRoleAdmin.RoleName + ""));
                    TempData["MessageType"] = "danger";
                    TempData["Message"] = "There is a problem with you application. Please try again or contact the system administrator";
                    return RedirectToAction("CompanyInformation");
                    #endregion
                }
                membershipService.CreateUser(OrgObj.TINNumber, tempUser.Password, model.organisation.ContactInfo.FirstOrDefault().EmailAddress, null, null, true, out createStatus);
                
                var user = swdb.Users.Where(x => x.UserName == OrgObj.TINNumber).FirstOrDefault();
                if(user ==null)
                {
                    //throw expection
                    #region show error
                    Elmah.ErrorSignal.FromCurrentContext().Raise(new Exception("Cannot find Organisation user membershipService.CreateUser"));
                    TempData["MessageType"] = "danger";
                    TempData["Message"] = "There is a problem with you application. Please try again or contact the system administrator";
                    return RedirectToAction("CompanyInformation");
                    #endregion
                }
                if (createStatus == MembershipCreateStatus.Success)
                {

                   
                    user.Roles.Add(GetRoleAdmin);
                    foreach (var cat in CategoryUsed)
                    {
                        if (cat.Role != null)
                        {
                            var Caterole = cat.Role.Split(',');
                            for (var i = 0; i < Caterole.Length; i++)
                            {
                                var rolename = Caterole[i];
                                var GetRole = swdb.Roles.Where(x => x.RoleName == rolename).FirstOrDefault();
                                if(GetRole==null)
                                {
                                    #region show error
                                    Elmah.ErrorSignal.FromCurrentContext().Raise(new Exception("Cannot find the role name "+rolename+" to add to org Role"));
                                    TempData["MessageType"] = "danger";
                                    TempData["Message"] = "There is a problem with you application. Please try again or contact the system administrator";
                                    return RedirectToAction("CompanyInformation");
                                    #endregion
                                }
                                OrgObj.Roles.Add(GetRole);
                                swdb.SaveChanges();                           
                            }
                        }
                    }


                    //add organisation user and 
                    OrgObj.Users.Add(user);
                  //  Contact.Users.Add(user);


                    ////Create Table Object
                    sw.ApprovalRegistration applicant = new sw.ApprovalRegistration()
                    {
                        ModifiedBy = "System",
                        ModifiedDate = now,
                        OwnedBy = ownedBy,
                        WorkFlowId = workflowId,
                        DateApplied = now,
                        OrganizationId = OrgObj.Id
                    };
                    swdb.ApprovalRegistration.AddObject(applicant);

                    /// Assign Trader Process Status
                                     
                    applicant.RegStatus.Add(status);

                 
                    swdb.SaveChanges();
                    formsAuthenticationService.SetAuthCookie(OrgObj.TINNumber, false);
                    #region ///////////// LoG Notification///////////////////////
                    var alert = swdb.Alert.FirstOrDefault(x => x.Id == 3);
                    if (alert != null)
                    {
                        //Send SMS///
                        var subject = alert.SubjectSms;
                        var message = alert.Sms;
                        var receiver = Contact.MobileNumber;
                        message = message.Replace("%Company_Name%", OrgObj.Name);
                        message = message.Replace("%Username%", OrgObj.TINNumber);
                        message = message.Replace("%Password%", tempUser.Password);
                        @SaveNotification("GRA-GTH", receiver, subject, message, 1);
                        receiver = Contact.MobileNumber;
                        @SaveNotification("GRA-GTH", receiver, subject, message, 1);
                        //Send Email//
                        subject = alert.SubjectEmail;
                        receiver = Contact.EmailAddress;
                        message = alert.Email;
                        message = message.Replace("%Company_Name%", OrgObj.Name);
                        message = message.Replace("%Username%", OrgObj.TINNumber);
                        message = message.Replace("%Password%", tempUser.Password);
                        @SaveNotification(Properties.Settings.Default.NoReplyEmail, receiver, subject, message, 2);
                        receiver = Contact.EmailAddress;
                        @SaveNotification(Properties.Settings.Default.NoReplyEmail, receiver, subject, message, 2);
                        //  /////////////////////////////////////////////////////

                    }
                    #endregion
                    model.Id = Id;
                    return RedirectToAction("Feedback", new { Id = model.Id });                  
                }
                else
                {
                    TempData["messageType"] = "danger";
                    TempData["message"] = "This is the error that occur "+ ErrorCodeToString(createStatus)+ " Please try again or contact the system administrator";
                    //ModelState.AddModelError("", ErrorCodeToString(createStatus));
                    return RedirectToAction("Review", new { Id = model.Id });
                }
               // return RedirectToAction("Review", new { Id = model.Id });
            }
            catch(Exception ex)
            {
                TempData["messageType"] = "danger";
                TempData["message"] = "An Error occur. Please try again later or contact the system administrator";
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                return RedirectToAction("CompanyInformation");
            }
        }
        #endregion

        [HttpGet]
        [OutputCache(NoStore =true, Duration =0, VaryByParam ="None")]
        public ActionResult Feedback(string Id=null)
        {
            try
            {
                if (Id == null)
                {

                    Elmah.ErrorSignal.FromCurrentContext().Raise(new Exception("No TIN sent to the company address Page: Company Registration"));
                    TempData["MessageType"] = "danger";
                    TempData["Message"] = "There is a problem with you application. Please try again or contact the system administrator";
                    return RedirectToAction("CompanyInformation");
                }
                StakeholderRegistrationModel model = new StakeholderRegistrationModel();
                var OrgObj = swdb.Organisation.Where(x => x.TINNumber == Id).FirstOrDefault();             
                var Approval = OrgObj.ApprovalRegistration.FirstOrDefault();

                //if (Approval != null)
                //{
                //    TempData["MessageType"] = "danger";
                //    TempData["Message"] = "You have already submitted your registration. Please login to proceed or contact the systm administrator";
                //    return RedirectToAction("CompanyInformation");
                //}

                var tempUser = db.TempUser.Where(x => x.Tin == Id).FirstOrDefault();
                if (tempUser != null)
                {
                    //throw expection
                    #region Delete User login from temp table    
                    TempUser del = new TempUser { };
                    db.TempUser.DeleteObject(tempUser);
                    db.SaveChanges();
                    #endregion
                }
                string rawFeedback = (from m in swdb.MessageTemplate where m.Id==5 select m.Body).FirstOrDefault();
                TempData["Feedback"] = rawFeedback.Replace("%Company_Name%", OrgObj.Name);
                model.Id = Id;
                return View(model);

            }
            catch (Exception ex)
            {
                TempData["messageType"] = "danger";
                TempData["message"] = "An Error occur. Please try again later or contact the system administrator";
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                return RedirectToAction("CompanyInformation");
            }
        }

        protected string GetApprovalProcessOwnedBy()
        {

            try
            {
              
                return Properties.Settings.Default.UserRegistrationRole;
            }
            catch (Exception ex)
            //Module failed to load
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                //ModuleException.ProcessModuleLoadException(this, exc);
                //Skin.AddModuleMessage(this, exc.Message, ModuleMessage.ModuleMessageType.RedError);
                return null;
            }
        }


        #region Private Methods
        private bool SaveNotification(string sender, string receiver, string subject, string message, int type)
        {
            try
            {
                DateTime timestamp = DateTime.Now;
                sw.Notification row = new sw.Notification
                {
                    Sender = sender,
                    Receiver = receiver,
                    Subject = subject,
                    Message = message,
                    NotificationStatusId = 0,
                    NotificationTypeId = type,
                    SentDate = timestamp,
                    ModifiedDate = timestamp,
                    ModifiedBy = "System"
                };
                swdb.Notification.AddObject(row);
                swdb.SaveChanges();

                // Send(row.Receiver);
                return true;
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                return false;
            }
        }
        #endregion



    }
}
