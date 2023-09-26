using Elmah;
using GNSW.DAL;
using Newtonsoft.Json;
using Project.DAL;
using Project.Models;
using Project.Properties;
using System;
using System.Collections.Generic;
using System.Data.Objects;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Project.Controllers
{
    public class EServicesController : Controller
    {
        private PROEntities db = new PROEntities();

        private GNSWEntities swdb = new GNSWEntities();

        public EServicesController()
        {
        }

        public ActionResult AgencyFees(int Id = 0)
        {
            ActionResult action;
            try
            {
                EServicesViewModel eServicesViewModel = new EServicesViewModel();
                if (Id != 0)
                {
                    eServicesViewModel.organisation = (
                        from x in this.db.Organization
                        where x.Id == Id
                        select x).FirstOrDefault<Organization>();
                    eServicesViewModel.organisationFee = eServicesViewModel.organisation.OrganizationFee.ToList<OrganizationFee>();
                    eServicesViewModel.orgId = (
                        from o in this.db.OrganizationFee
                        orderby o.Organization.Name
                        select o.OrganizationId).ToList<int>();
                    action = base.View(eServicesViewModel);
                }
                else
                {
                    eServicesViewModel.OrgFees = (
                        from x in this.db.OrganizationFee
                        orderby x.Organization.Name
                        select x).FirstOrDefault<OrganizationFee>();
                    eServicesViewModel.organisation = eServicesViewModel.OrgFees.Organization;
                    eServicesViewModel.organisationFee = eServicesViewModel.organisation.OrganizationFee.ToList<OrganizationFee>();
                    eServicesViewModel.orgId = (
                        from o in this.db.OrganizationFee
                        orderby o.Organization.Name
                        select o.OrganizationId).ToList<int>();
                    action = base.View(eServicesViewModel);
                }
            }
            catch (Exception exception1)
            {
                Exception exception = exception1;
                base.TempData["messageType"] = "alert-danger";
                ErrorSignal.FromCurrentContext().Raise(exception);
                action = base.RedirectToAction("Error404", "Home", new { area = "" });
            }
            return action;
        }

        public ActionResult AgencyFeesDetails(int Id = 0)
        {
            ActionResult action;
            try
            {
                EServicesViewModel eServicesViewModel = new EServicesViewModel();
                if (Id != 0)
                {
                    eServicesViewModel.organisation = (
                        from x in this.db.Organization
                        where x.Id == Id
                        select x).FirstOrDefault<Organization>();
                    eServicesViewModel.organisationFee = eServicesViewModel.organisation.OrganizationFee.ToList<OrganizationFee>();
                    eServicesViewModel.orgId = (
                        from o in this.db.OrganizationFee
                        orderby o.Organization.Name
                        select o.OrganizationId).ToList<int>();
                    action = base.View(eServicesViewModel);
                }
                else
                {
                    eServicesViewModel.OrgFees = (
                        from x in this.db.OrganizationFee
                        orderby x.Organization.Name
                        select x).FirstOrDefault<OrganizationFee>();
                    eServicesViewModel.organisation = eServicesViewModel.OrgFees.Organization;
                    eServicesViewModel.organisationFee = eServicesViewModel.organisation.OrganizationFee.ToList<OrganizationFee>();
                    eServicesViewModel.orgId = (
                        from o in this.db.OrganizationFee
                        orderby o.Organization.Name
                        select o.OrganizationId).ToList<int>();
                    action = base.View(eServicesViewModel);
                }
            }
            catch (Exception exception1)
            {
                Exception exception = exception1;
                base.TempData["messageType"] = "alert-danger";
                ErrorSignal.FromCurrentContext().Raise(exception);
                action = base.RedirectToAction("Error404", "Home", new { area = "" });
            }
            return action;
        }

        public ActionResult AgencyFeesList()
        {

            ActionResult action;
            try
            {
                EServicesViewModel eServicesViewModel = new EServicesViewModel()
                {
                    OrgFees = (from x in this.db.OrganizationFee orderby x.Organization.Name select x).FirstOrDefault(),
                    //organisation = eServicesViewModel.OrgFees.Organization,
                   // organisationFee = eServicesViewModel.organisation.OrganizationFee.ToList(),
                    orgId = (from o in this.db.OrganizationFee orderby o.Organization.Name select o.OrganizationId).ToList()
                };
                action = base.View(eServicesViewModel);
            }
            catch (Exception exception1)
            {
                Exception exception = exception1;
                base.TempData["messageType"] = "alert-danger";
                ErrorSignal.FromCurrentContext().Raise(exception);
                action = base.RedirectToAction("Error404", "Home", new { area = "" });
            }
            return action;
        }

        public ActionResult AutoSparePartImportProcess(int Id = 3)
        {
            ActionResult action;
            try
            {
                EServicesViewModel eServicesViewModel = new EServicesViewModel();
                ImportExportProcedure importExportProcedure = (
                    from x in this.db.ImportExportProcedure
                    where x.Id == Id
                    select x).FirstOrDefault<ImportExportProcedure>();
                eServicesViewModel.importexportProceedure = importExportProcedure;
                action = base.View(eServicesViewModel);
            }
            catch (Exception exception1)
            {
                Exception exception = exception1;
                base.TempData["messageType"] = "alert-danger";
                ErrorSignal.FromCurrentContext().Raise(exception);
                action = base.RedirectToAction("Error404", "Home", new { area = "" });
            }
            return action;
        }

        public ActionResult CashewNutsExportProcedure(int Id = 7)
        {
            ActionResult action;
            try
            {
                EServicesViewModel eServicesViewModel = new EServicesViewModel();
                ImportExportProcedure importExportProcedure = (
                    from x in this.db.ImportExportProcedure
                    where x.Id == Id
                    select x).FirstOrDefault<ImportExportProcedure>();
                eServicesViewModel.importexportProceedure = importExportProcedure;
                action = base.View(eServicesViewModel);
            }
            catch (Exception exception1)
            {
                Exception exception = exception1;
                base.TempData["messageType"] = "alert-danger";
                ErrorSignal.FromCurrentContext().Raise(exception);
                action = base.RedirectToAction("Error404", "Home", new { area = "" });
            }
            return action;
        }

        public ActionResult CocoaBeanExportProcedure(int Id = 8)
        {
            ActionResult action;
            try
            {
                EServicesViewModel eServicesViewModel = new EServicesViewModel();
                ImportExportProcedure importExportProcedure = (
                    from x in this.db.ImportExportProcedure
                    where x.Id == Id
                    select x).FirstOrDefault<ImportExportProcedure>();
                eServicesViewModel.importexportProceedure = importExportProcedure;
                action = base.View(eServicesViewModel);
            }
            catch (Exception exception1)
            {
                Exception exception = exception1;
                base.TempData["messageType"] = "alert-danger";
                ErrorSignal.FromCurrentContext().Raise(exception);
                action = base.RedirectToAction("Error404", "Home", new { area = "" });
            }
            return action;
        }

        public ActionResult ConsigmentTracking()
        {
            ActionResult action;
            try
            {
                action = base.View(new EServicesViewModel());
            }
            catch (Exception exception1)
            {
                Exception exception = exception1;
                base.TempData["messageType"] = "alert-danger";
                ErrorSignal.FromCurrentContext().Raise(exception);
                action = base.RedirectToAction("Error404", "Home", new { area = "" });
            }
            return action;
        }

        //[HttpPost]
        //public ActionResult ConsigmentTracking(EServicesViewModel model)
        //{
        //    ActionResult action;
        //    try
        //    {
        //        if (base.ModelState.IsValid)
        //        {
        //            if (this.IsTinValid(model.MobileTrackingform.Tin))
        //            {
        //                using (HttpClient httpClient = new HttpClient())
        //                {
        //                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(Encoding.ASCII.GetBytes("MobileTracker:1004bd6a-3c6f-4c25-bb12-55b25d5bbc40")));
        //                    HttpResponseMessage result = httpClient.PostAsync(Settings.Default.MobileTrackerURL.TrimEnd(new char[] { '/' }), new StringContent(string.Concat(new string[] { "{\"UcrNumber\":\"", model.MobileTrackingform.UCRNo, "\",\"TIN\":\"", model.MobileTrackingform.Tin, "\"}" }), Encoding.UTF8, "application/json")).Result;
        //                    if (result.IsSuccessStatusCode)
        //                    {
        //                        rootObject rootObject = result.Content.ReadAsAsync<RootObject>().Result;
        //                        if (rootObject.status == 200)
        //                        {
        //                            if (rootObject.responseDetail.successDetail == null)
        //                            {
        //                                base.TempData["MessageType"] = "danger";
        //                                base.TempData["Message"] = "Something went wrong: Please check that you entered the correct URC Number.";
        //                                action = base.View(model);
        //                                return action;
        //                            }
        //                            else
        //                            {
        //                                model.UCRList = rootObject.responseDetail.successDetail.ToList<SuccessDetail>();
        //                                model.hasSearch = true;
        //                                DutyCounter totalUsed = (
        //                                    from x in this.db.DutyCounter
        //                                    where x.CounterType == "Consigment Tracker"
        //                                    select x).FirstOrDefault<DutyCounter>();
        //                                totalUsed.TotalUsed = totalUsed.TotalUsed + 1;
        //                                this.db.SaveChanges();
        //                                action = base.View(model);
        //                                return action;
        //                            }
        //                        }
        //                    }
        //                }
        //            }
        //            else
        //            {
        //                base.TempData["MessageType"] = "danger";
        //                base.TempData["Message"] = "Verification Failed: Please check that you entered the correct Company TIN.";
        //                action = base.View(model);
        //                return action;
        //            }
        //        }
        //        action = base.View(model);
        //    }
        //    catch (Exception exception1)
        //    {
        //        Exception exception = exception1;
        //        base.TempData["messageType"] = "alert-danger";
        //        ErrorSignal.FromCurrentContext().Raise(exception);
        //        action = base.RedirectToAction("Error404", "Home", new { area = "" });
        //    }
        //    return action;
        //}

        public ActionResult DocumentsList(int Id)
        {
            ActionResult action;
            try
            {
                if (Id != 0)
                {
                    EServicesViewModel eServicesViewModel = new EServicesViewModel()
                    {
                        DocumentInfoList = (
                            from x in this.db.DocumentInfo
                            where x.DocumentCategoryId == Id && (x.IsValid == (bool?)true || x.IsValid == null)
                            orderby x.Id descending
                            select x).ToList(),
                        DocumentCategoryList = (
                            from x in this.db.DocumentCategory
                            orderby x.Name
                            select x).ToList<DocumentCategory>()
                    };
                    DocumentCategory documentCategory = (
                        from x in this.db.DocumentCategory
                        where x.Id == Id
                        select x).FirstOrDefault<DocumentCategory>();
                    eServicesViewModel.CategoryName = documentCategory.Name;
                    action = base.View(eServicesViewModel);
                }
                else
                {
                    base.TempData["messageType"] = "alert-danger";
                    ErrorSignal.FromCurrentContext().Raise(new Exception("Cannot find Vehicle Category Id. Freight Rate Page"));
                    action = base.RedirectToAction("Error404", "Home", new { area = "" });
                }
            }
            catch (Exception exception1)
            {
                Exception exception = exception1;
                base.TempData["messageType"] = "alert-danger";
                ErrorSignal.FromCurrentContext().Raise(exception);
                action = base.RedirectToAction("Error404", "Home", new { area = "" });
            }
            return action;
        }

        public ActionResult ExportProcess(int Id = 11)
        {
            ActionResult action;
            try
            {
                EServicesViewModel eServicesViewModel = new EServicesViewModel();
                ImportExportProcedure importExportProcedure = (
                    from x in this.db.ImportExportProcedure
                    where x.Id == Id
                    select x).FirstOrDefault<ImportExportProcedure>();
                eServicesViewModel.importexportProceedure = importExportProcedure;
                action = base.View(eServicesViewModel);
            }
            catch (Exception exception1)
            {
                Exception exception = exception1;
                base.TempData["messageType"] = "alert-danger";
                ErrorSignal.FromCurrentContext().Raise(exception);
                action = base.RedirectToAction("Error404", "Home", new { area = "" });
            }
            return action;
        }

        public ActionResult ExportSpecificCommodities(int Id = 6)
        {
            ActionResult action;
            try
            {
                EServicesViewModel eServicesViewModel = new EServicesViewModel();
                ImportExportProcedure importExportProcedure = (
                    from x in this.db.ImportExportProcedure
                    where x.Id == Id
                    select x).FirstOrDefault<ImportExportProcedure>();
                eServicesViewModel.importexportProceedure = importExportProcedure;
                action = base.View(eServicesViewModel);
            }
            catch (Exception exception1)
            {
                Exception exception = exception1;
                base.TempData["messageType"] = "alert-danger";
                ErrorSignal.FromCurrentContext().Raise(exception);
                action = base.RedirectToAction("Error404", "Home", new { area = "" });
            }
            return action;
        }

        public ActionResult FreightRates(EServicesViewModel model)
        {
            ActionResult action;
            try
            {
                model.CountryList = (
                    from s in this.db.Country
                    join c in this.db.FreightRate on s.Id equals c.CountryId
                    select new IntegerSelectListItem()
                    {
                        Text = s.Name,
                        Value = s.Id
                    } into x
                    orderby x.Text
                    select x).ToList<IntegerSelectListItem>();
                model.VehicleTypeList = (
                    from s in this.db.VehicleType
                    select new IntegerSelectListItem()
                    {
                        Text = s.Name,
                        Value = s.CategoryId
                    } into x
                    orderby x.Text
                    select x).ToList<IntegerSelectListItem>();
                action = base.View(model);
            }
            catch (Exception exception1)
            {
                Exception exception = exception1;
                base.TempData["messageType"] = "alert-danger";
                ErrorSignal.FromCurrentContext().Raise(exception);
                action = base.RedirectToAction("Error404", "Home", new { area = "" });
            }
            return action;
        }

        [HttpPost]
        public ActionResult FreightRatesResult(EServicesViewModel model)
        {
            ActionResult action;
            try
            {
                if (model.CountryId != 0)
                {
                    model.FreightRateList = (
                        from f in this.db.FreightRate
                        where f.CountryId == model.CountryId
                        select f).ToList<FreightRate>();
                    var country = (from c in this.db.Country
                        where c.Id == model.CountryId
                        select c).FirstOrDefault();
                    model.CountryName = country.Name;
                    action = base.View(model);
                }
                else
                {
                    base.TempData["messageType"] = "alert-danger";
                    ErrorSignal.FromCurrentContext().Raise(new Exception("Cannot find Country Id. Freight Rate Page"));
                    action = base.RedirectToAction("Error404", "Home", new { area = "" });
                }
            }
            catch (Exception exception1)
            {
                Exception exception = exception1;
                base.TempData["messageType"] = "alert-danger";
                ErrorSignal.FromCurrentContext().Raise(exception);
                action = base.RedirectToAction("Error404", "Home", new { area = "" });
            }
            return action;
        }

        public ActionResult GeneralGoods()
        {
            ActionResult action;
            try
            {
                EServicesViewModel eServicesViewModel = new EServicesViewModel()
                {
                    CountryList = (
                        from s in this.db.Country
                        join c in this.db.FreightRate on s.Id equals c.CountryId
                        select new IntegerSelectListItem()
                        {
                            Text = s.Name,
                            Value = s.Id
                        } into x
                        orderby x.Text
                        select x).Distinct<IntegerSelectListItem>().ToList<IntegerSelectListItem>(),
                    VehicleTypeList = (
                        from s in this.db.VehicleType
                        select new IntegerSelectListItem()
                        {
                            Text = s.Name,
                            Value = s.CategoryId
                        } into x
                        orderby x.Text
                        select x).ToList<IntegerSelectListItem>()
                };
                action = base.View(eServicesViewModel);
            }
            catch (Exception exception1)
            {
                Exception exception = exception1;
                base.TempData["messageType"] = "alert-danger";
                ErrorSignal.FromCurrentContext().Raise(exception);
                action = base.RedirectToAction("Error404", "Home", new { area = "" });
            }
            return action;
        }

        [HttpPost]
        public ActionResult GeneralGoods(EServicesViewModel model)
        {
            ActionResult action;
            try
            {
                if (model.CountryId != 0)
                {
                    model.CountryList = (
                        from s in this.db.Country
                        join c in this.db.FreightRate on s.Id equals c.CountryId
                        select new IntegerSelectListItem()
                        {
                            Text = s.Name,
                            Value = s.Id
                        } into x
                        orderby x.Text
                        select x).Distinct<IntegerSelectListItem>().ToList<IntegerSelectListItem>();
                    model.FreightRateList = (
                        from f in this.db.FreightRate
                        where f.CountryId == model.CountryId
                        select f).ToList<FreightRate>();
                    var country = (from c in this.db.Country where c.Id == model.CountryId select c).FirstOrDefault();
                    model.CountryName = country.Name;
                    model.hasSearch = true;
                    action = base.View(model);
                }
                else
                {
                    base.TempData["messageType"] = "alert-danger";
                    ErrorSignal.FromCurrentContext().Raise(new Exception("Cannot find Country Id. Freight Rate Page"));
                    action = base.RedirectToAction("Error404", "Home", new { area = "" });
                }
            }
            catch (Exception exception1)
            {
                Exception exception = exception1;
                base.TempData["messageType"] = "alert-danger";
                ErrorSignal.FromCurrentContext().Raise(exception);
                action = base.RedirectToAction("Error404", "Home", new { area = "" });
            }
            return action;
        }

        public ActionResult GoldExportProcedure(int Id = 9)
        {
            ActionResult action;
            try
            {
                EServicesViewModel eServicesViewModel = new EServicesViewModel();
                ImportExportProcedure importExportProcedure = (
                    from x in this.db.ImportExportProcedure
                    where x.Id == Id
                    select x).FirstOrDefault<ImportExportProcedure>();
                eServicesViewModel.importexportProceedure = importExportProcedure;
                action = base.View(eServicesViewModel);
            }
            catch (Exception exception1)
            {
                Exception exception = exception1;
                base.TempData["messageType"] = "alert-danger";
                ErrorSignal.FromCurrentContext().Raise(exception);
                action = base.RedirectToAction("Error404", "Home", new { area = "" });
            }
            return action;
        }

        public ActionResult GovernanceStructure()
        {
            ActionResult action;
            try
            {
                action = base.View();
            }
            catch (Exception exception1)
            {
                Exception exception = exception1;
                base.TempData["messageType"] = "alert-danger";
                ErrorSignal.FromCurrentContext().Raise(exception);
                action = base.RedirectToAction("Error404", "Home", new { area = "" });
            }
            return action;
        }

        public ActionResult ImportProcess(int Id = 10)
        {
            ActionResult action;
            try
            {
                EServicesViewModel eServicesViewModel = new EServicesViewModel();
                ImportExportProcedure importExportProcedure = (
                    from x in this.db.ImportExportProcedure
                    where x.Id == Id
                    select x).FirstOrDefault<ImportExportProcedure>();
                eServicesViewModel.importexportProceedure = importExportProcedure;
                action = base.View(eServicesViewModel);
            }
            catch (Exception exception1)
            {
                Exception exception = exception1;
                base.TempData["messageType"] = "alert-danger";
                ErrorSignal.FromCurrentContext().Raise(exception);
                action = base.RedirectToAction("Error404", "Home", new { area = "" });
            }
            return action;
        }

        public ActionResult ImportSpecificCommodities(int Id = 1)
        {
            ActionResult action;
            try
            {
                EServicesViewModel eServicesViewModel = new EServicesViewModel();
                ImportExportProcedure importExportProcedure = (
                    from x in this.db.ImportExportProcedure
                    where x.Id == Id
                    select x).FirstOrDefault<ImportExportProcedure>();
                eServicesViewModel.importexportProceedure = importExportProcedure;
                action = base.View(eServicesViewModel);
            }
            catch (Exception exception1)
            {
                Exception exception = exception1;
                base.TempData["messageType"] = "alert-danger";
                ErrorSignal.FromCurrentContext().Raise(exception);
                action = base.RedirectToAction("Error404", "Home", new { area = "" });
            }
            return action;
        }

        public static bool IsDateTime(string txtDate)
        {
            DateTime dateTime;
            return DateTime.TryParse(txtDate, out dateTime);
        }

        private static bool IsInteger(string str)
        {
            int num;
            return int.TryParse(str, out num);
        }

        public bool IsTinValid(string tin)
        {
            bool flag = true;
            using (HttpClient httpClient = new HttpClient())
            {
                string str = tin;
                string masterDataUrl = Settings.Default.MasterDataUrl;
                string str1 = string.Concat("TaxIdentificationNumber?tin=", str.Trim());
                string str2 = string.Concat(masterDataUrl, str1);
                Console.WriteLine(string.Concat("GET: + ", str2));
                byte[] bytes = Encoding.ASCII.GetBytes(string.Concat(Settings.Default.Username, ":", Settings.Default.Key));
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(bytes));
                Task<HttpResponseMessage> async = httpClient.GetAsync(str2);
                async.Wait();
                HttpResponseMessage result = async.Result;
                if (result.IsSuccessStatusCode && string.IsNullOrEmpty(JsonConvert.DeserializeObject<TinVerificationObj>(result.Content.ReadAsStringAsync().Result).CompanyName))
                {
                    flag = false;
                }
            }
            return flag;
        }

        public ActionResult MarineTraffic()
        {
            return base.View();
        }

        public ActionResult MarketPlace(EServicesViewModel model)
        {
            ActionResult action;
            try
            {
                model.ProductTypeList = (
                    from x in this.db.ProductType
                    orderby x.Name
                    select x).ToList<ProductType>();
                action = base.View(model);
            }
            catch (Exception exception1)
            {
                Exception exception = exception1;
                base.TempData["messageType"] = "alert-danger";
                ErrorSignal.FromCurrentContext().Raise(exception);
                action = base.RedirectToAction("Error404", "Home", new { area = "" });
            }
            return action;
        }

        public ActionResult MarketPlaceContact(int ProductTypeId = 0, int ProductId = 0)
        {
            ActionResult action;
            try
            {
                EServicesViewModel eServicesViewModel = new EServicesViewModel();
                if (ProductTypeId != 0 || ProductId != 0)
                {
                    eServicesViewModel.ProductTypeList = (
                        from x in this.db.ProductType
                        orderby x.Name
                        select x).ToList<ProductType>();
                    eServicesViewModel.ProductList = (
                        from x in this.db.Product
                        where x.ProductTypeId == ProductTypeId
                        orderby x.Name
                        select x).ToList<Product>();
                    eServicesViewModel.ExporterProductList = (
                        from x in this.db.ExporterProduct
                        where x.ProductTypeId == ProductTypeId && x.ProductId == ProductId
                        orderby x.ExporterName
                        select x).ToList<ExporterProduct>();
                    eServicesViewModel.product = (
                        from x in this.db.Product
                        where x.Id == ProductId
                        select x).FirstOrDefault<Product>();
                    eServicesViewModel.ProductName = (new CultureInfo("en-US")).TextInfo.ToTitleCase(eServicesViewModel.product.Name);
                    int num = 1;
                    ProductType noOfView = (
                        from x in this.db.ProductType
                        where x.Id == ProductTypeId
                        select x).FirstOrDefault<ProductType>();
                    noOfView.NoOfView = num + noOfView.NoOfView;
                    this.db.SaveChanges();
                    action = base.View(eServicesViewModel);
                }
                else
                {
                    ProductType productType = (
                        from x in this.db.ProductType
                        orderby x.Name
                        select x).FirstOrDefault<ProductType>();
                    Product product = (
                        from x in this.db.Product
                        where x.ProductTypeId == productType.Id
                        orderby x.Name
                        select x).FirstOrDefault<Product>();
                    eServicesViewModel.product = (
                        from x in this.db.Product
                        where x.Id == product.Id
                        orderby x.Name
                        select x).FirstOrDefault<Product>();
                    eServicesViewModel.ExporterProductList = (
                        from x in this.db.ExporterProduct
                        where x.ProductTypeId == productType.Id && x.ProductId == product.Id
                        orderby x.ExporterName
                        select x).Distinct<ExporterProduct>().ToList<ExporterProduct>();
                    eServicesViewModel.ProductTypeList = (
                        from x in this.db.ProductType
                        orderby x.Name
                        select x).ToList<ProductType>();
                    eServicesViewModel.ProductList = (
                        from x in this.db.Product
                        where x.ProductTypeId == productType.Id
                        orderby x.Name
                        select x).ToList<Product>();
                    eServicesViewModel.ProductName = (new CultureInfo("en-US")).TextInfo.ToTitleCase(eServicesViewModel.product.Name);
                    action = base.View(eServicesViewModel);
                }
            }
            catch (Exception exception1)
            {
                Exception exception = exception1;
                base.TempData["messageType"] = "alert-danger";
                ErrorSignal.FromCurrentContext().Raise(exception);
                action = base.RedirectToAction("Error404", "Home", new { area = "" });
            }
            return action;
        }

        public ActionResult MedicineInDosesImportProcess(int Id = 2)
        {
            ActionResult action;
            try
            {
                EServicesViewModel eServicesViewModel = new EServicesViewModel();
                ImportExportProcedure importExportProcedure = (
                    from x in this.db.ImportExportProcedure
                    where x.Id == Id
                    select x).FirstOrDefault<ImportExportProcedure>();
                eServicesViewModel.importexportProceedure = importExportProcedure;
                action = base.View(eServicesViewModel);
            }
            catch (Exception exception1)
            {
                Exception exception = exception1;
                base.TempData["messageType"] = "alert-danger";
                ErrorSignal.FromCurrentContext().Raise(exception);
                action = base.RedirectToAction("Error404", "Home", new { area = "" });
            }
            return action;
        }

        public ActionResult PhysicaExaminationTool(EServicesViewModel model)
        {
            ActionResult action;
            try
            {
                model.Examination = this.db.PhysicalExamination.ToList<PhysicalExamination>();
                model.DownloadPath = Settings.Default.DownloadPath;
                action = base.View(model);
            }
            catch (Exception exception1)
            {
                Exception exception = exception1;
                base.TempData["messageType"] = "alert-danger";
                ErrorSignal.FromCurrentContext().Raise(exception);
                action = base.RedirectToAction("Error404", "Home", new { area = "" });
            }
            return action;
        }

        public ActionResult ReadOnline(int Id)
        {
            ActionResult action;
            try
            {
                if (Id != 0)
                {
                    EServicesViewModel eServicesViewModel = new EServicesViewModel()
                    {
                        Documentinfo = (
                            from x in this.db.DocumentInfo
                            where x.Id == Id
                            select x).FirstOrDefault()
                    };
                    action = base.View(eServicesViewModel);
                }
                else
                {
                    base.TempData["messageType"] = "alert-danger";
                    ErrorSignal.FromCurrentContext().Raise(new Exception("Cannot find Vehicle Category Id. Freight Rate Page"));
                    action = base.RedirectToAction("Error404", "Home", new { area = "" });
                }
            }
            catch (Exception exception1)
            {
                Exception exception = exception1;
                base.TempData["messageType"] = "alert-danger";
                ErrorSignal.FromCurrentContext().Raise(exception);
                action = base.RedirectToAction("Error404", "Home", new { area = "" });
            }
            return action;
        }

        public JsonResult Search(DataTableModel Model)
        {
            IEnumerable<OrganizationFee> organizationFee;
            if (string.IsNullOrEmpty(Model.sSearch))
            {
                organizationFee =
                    from x in this.db.OrganizationFee
                    where x.Organization.Name.Contains(Model.sSearch) || x.ProcessName.Contains(Model.sSearch)
                    select x;
            }
            else
            {
                bool flag = EServicesController.IsDateTime(Model.sSearch);
                if (EServicesController.IsInteger(Model.sSearch))
                {
                    int.Parse(Model.sSearch);
                }
                organizationFee = (!flag ?
                    from x in this.db.OrganizationFee
                    where x.Organization.Name.Contains(Model.sSearch) || x.ProcessName.Contains(Model.sSearch)
                    select x :
                    from x in this.db.OrganizationFee
                    where x.Organization.Name.Contains(Model.sSearch) || x.ProcessName.Contains(Model.sSearch)
                    select x);
            }
            int num = this.db.OrganizationFee.Count<OrganizationFee>();
            int num1 = Convert.ToInt32(base.Request["iSortCol_0"]);
            string item = base.Request["sSortDir_0"];
            if (num1 == 0 && item == "asc")
            {
                organizationFee =
                    from x in organizationFee
                    orderby x.ProcessName
                    select x;
            }
            else if (num1 == 0)
            {
                organizationFee =
                    from x in organizationFee
                    orderby x.ProcessName descending
                    select x;
            }
            else if (num1 == 1 && item == "asc")
            {
                organizationFee =
                    from x in organizationFee
                    orderby x.ProcessName
                    select x;
            }
            else if (num1 == 1)
            {
                organizationFee =
                    from x in organizationFee
                    orderby x.ProcessName descending
                    select x;
            }
            else if (num1 == 2 && item == "asc")
            {
                organizationFee =
                    from x in organizationFee
                    orderby x.Id
                    select x;
            }
            else if (num1 == 2)
            {
                organizationFee =
                    from x in organizationFee
                    orderby x.Id descending
                    select x;
            }
            if (num1 == 3 && item == "asc")
            {
                organizationFee =
                    from x in organizationFee
                    orderby x.ProcessName
                    select x;
            }
            else if (num1 == 3)
            {
                organizationFee =
                    from x in organizationFee
                    orderby x.ProcessName descending
                    select x;
            }
            else if (num1 == 4 && item == "asc")
            {
                organizationFee =
                    from x in organizationFee
                    orderby x.Id
                    select x;
            }
            else if (num1 != 4)
            {
                organizationFee = (item != "asc" ?
                    from x in organizationFee
                    orderby x.ProcessName descending
                    select x :
                    from x in organizationFee
                    orderby x.ProcessName
                    select x);
            }
            else
            {
                organizationFee =
                    from x in organizationFee
                    orderby x.Id descending
                    select x;
            }
            var collection =
                from x in organizationFee.Skip<OrganizationFee>(Model.iDisplayStart).Take<OrganizationFee>(Model.iDisplayLength)
                select new { ProcessName = x.ProcessName, Fees = x.FeesPurpose, Dollar = x.AmountInDollar, Cedis = x.AmountIdCedis };
            return base.Json(new { sEcho = Model.sEcho, iTotalRecords = num, iTotalDisplayRecords = organizationFee.Count<OrganizationFee>(), aaData = collection }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult SmartPorts()
        {
            ActionResult action;
            try
            {
                action = base.View();
            }
            catch (Exception exception1)
            {
                Exception exception = exception1;
                base.TempData["messageType"] = "alert-danger";
                ErrorSignal.FromCurrentContext().Raise(exception);
                action = base.RedirectToAction("Error404", "Home", new { area = "" });
            }
            return action;
        }

        public ActionResult TradeRegistration(int Id = 0)
        {
            ActionResult action;
            try
            {
                if (Id != 0)
                {
                    EServicesViewModel eServicesViewModel = new EServicesViewModel();
                    MessageTemplate messageTemplate = (
                        from x in this.swdb.MessageTemplate
                        where x.WorkFlowId == Id
                        select x).FirstOrDefault<MessageTemplate>();
                    eServicesViewModel.msg = messageTemplate;
                    eServicesViewModel.TraderRegistrationId = Id;
                    string str = (
                        from m in this.swdb.MessageTemplate
                        where m.WorkFlowId == Id && m.MessageTypeId == 5
                        select m.Body).FirstOrDefault<string>();
                    base.TempData["Guideline"] = str.Replace("%WorkFlowName%", messageTemplate.ApprovalWorkFlow.ProcessName);
                    action = base.View(eServicesViewModel);
                }
                else
                {
                    base.TempData["messageType"] = "alert-danger";
                    ErrorSignal.FromCurrentContext().Raise(new Exception("Cannot find Vehicle Category Id. Freight Rate Page"));
                    action = base.RedirectToAction("Error404", "Home", new { area = "" });
                }
            }
            catch (Exception exception1)
            {
                Exception exception = exception1;
                base.TempData["messageType"] = "danger";
                base.TempData["message"] = "An Error occur. Please try again later or contact the system administrator";
                ErrorSignal.FromCurrentContext().Raise(exception);
                action = base.RedirectToAction("Error404", "Home", new { area = "" });
            }
            return action;
        }

        public ActionResult TransitProcess(int Id = 12)
        {
            ActionResult action;
            try
            {
                EServicesViewModel eServicesViewModel = new EServicesViewModel();
                ImportExportProcedure importExportProcedure = (
                    from x in this.db.ImportExportProcedure
                    where x.Id == Id
                    select x).FirstOrDefault<ImportExportProcedure>();
                eServicesViewModel.importexportProceedure = importExportProcedure;
                action = base.View(eServicesViewModel);
            }
            catch (Exception exception1)
            {
                Exception exception = exception1;
                base.TempData["messageType"] = "alert-danger";
                ErrorSignal.FromCurrentContext().Raise(exception);
                action = base.RedirectToAction("Error404", "Home", new { area = "" });
            }
            return action;
        }

        public ActionResult UsedVehicleFreight()
        {
            ActionResult action;
            try
            {
                EServicesViewModel eServicesViewModel = new EServicesViewModel()
                {
                    VehicleTypeList = (
                        from s in this.db.VehicleType
                        select new IntegerSelectListItem()
                        {
                            Text = s.Name,
                            Value = s.CategoryId
                        } into x
                        orderby x.Text
                        select x).ToList<IntegerSelectListItem>()
                };
                action = base.View(eServicesViewModel);
            }
            catch (Exception exception1)
            {
                Exception exception = exception1;
                base.TempData["messageType"] = "alert-danger";
                ErrorSignal.FromCurrentContext().Raise(exception);
                action = base.RedirectToAction("Error404", "Home", new { area = "" });
            }
            return action;
        }

        [HttpPost]
        public ActionResult UsedVehicleFreight(EServicesViewModel model)
        {
            ActionResult action;
            try
            {
                if (model.VehicleCategoryId != 0)
                {
                    model.VehicleTypeList = (
                        from s in this.db.VehicleType
                        select new IntegerSelectListItem()
                        {
                            Text = s.Name,
                            Value = s.CategoryId
                        } into x
                        orderby x.Text
                        select x).ToList<IntegerSelectListItem>();
                    VehicleType vehicleType = (
                        from x in this.db.VehicleType
                        where x.CategoryId == model.VehicleCategoryId
                        select x).FirstOrDefault<VehicleType>();
                    model.FreightValueList = (
                        from f in this.db.FreightValue
                        where f.CategoryId == model.VehicleCategoryId
                        select f).ToList<FreightValue>();
                    model.OverageRatesList = (
                        from f in this.db.OverageRates
                        where f.CategoryId == model.VehicleCategoryId
                        select f).ToList<OverageRates>();
                    model.VehicleTypeName = vehicleType.VehicleCategory.Name;
                    model.hasSearch = true;
                    action = base.View(model);
                }
                else
                {
                    base.TempData["messageType"] = "alert-danger";
                    ErrorSignal.FromCurrentContext().Raise(new Exception("Cannot find Vehicle Category Id. Freight Rate Page"));
                    action = base.RedirectToAction("Error404", "Home", new { area = "" });
                }
            }
            catch (Exception exception1)
            {
                Exception exception = exception1;
                base.TempData["messageType"] = "alert-danger";
                ErrorSignal.FromCurrentContext().Raise(exception);
                action = base.RedirectToAction("Error404", "Home", new { area = "" });
            }
            return action;
        }

        public ActionResult VegetablePalmOilImportProcedure(int Id = 4)
        {
            ActionResult action;
            try
            {
                EServicesViewModel eServicesViewModel = new EServicesViewModel();
                ImportExportProcedure importExportProcedure = (
                    from x in this.db.ImportExportProcedure
                    where x.Id == Id
                    select x).FirstOrDefault<ImportExportProcedure>();
                eServicesViewModel.importexportProceedure = importExportProcedure;
                action = base.View(eServicesViewModel);
            }
            catch (Exception exception1)
            {
                Exception exception = exception1;
                base.TempData["messageType"] = "alert-danger";
                ErrorSignal.FromCurrentContext().Raise(exception);
                action = base.RedirectToAction("Error404", "Home", new { area = "" });
            }
            return action;
        }

        public ActionResult VehicleImportProcedure(int Id = 5)
        {
            ActionResult action;
            try
            {
                EServicesViewModel eServicesViewModel = new EServicesViewModel();
                ImportExportProcedure importExportProcedure = (
                    from x in this.db.ImportExportProcedure
                    where x.Id == Id
                    select x).FirstOrDefault<ImportExportProcedure>();
                eServicesViewModel.importexportProceedure = importExportProcedure;
                action = base.View(eServicesViewModel);
            }
            catch (Exception exception1)
            {
                Exception exception = exception1;
                base.TempData["messageType"] = "alert-danger";
                ErrorSignal.FromCurrentContext().Raise(exception);
                action = base.RedirectToAction("Error404", "Home", new { area = "" });
            }
            return action;
        }
    }
}