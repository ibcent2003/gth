
using Elmah;
using sw = GNSW.DAL;
using cha = CHAMS.DAL;
using PAARS.DAL;
using Project.DAL;
using Project.database_Access_Layer;
using Project.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Objects;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Web.Mvc;
using Classification.DAL;

namespace Project.Controllers
{
    public class DutyCalculatorController : Controller
    {
        public DutyCalculatorController()
        {

        }
        //
        // GET: /DutyCalculator/
        private cha.CHAMSEntities chasmdb = new cha.CHAMSEntities();
        private PAARv2Entities paardb = new PAARv2Entities();
        private PROEntities db = new PROEntities();
        private sw.GNSWEntities swdb = new sw.GNSWEntities();
        private ClassificationToolsEntities dbclass = new ClassificationToolsEntities(); 
        SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["con"].ConnectionString);
       

        database_Access_Layer.db dblayer = new database_Access_Layer.db();
        public ActionResult Index()
        {
            return View();
        }


        public ActionResult GeneralGood()
        {
            try
            {
                DutyCalculatorModel model = new DutyCalculatorModel()
                {
                    CurrencyList = new SelectList((
                       from x in this.paardb.ExchangeRateView
                       orderby x.Week descending
                       select x).Take<ExchangeRateView>(18), "CurrencyCode", "CurrencyName"),
                    resultform = new GeneralGoodsResult()
                };

                return View(model);
            }
            catch (Exception ex)
            {
                TempData["messageType"] = "danger";
                TempData["message"] = "An Error occur. Please try again later or contact the system administrator";
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                return RedirectToAction("Index");
            }

        }

       


        [HttpPost]
        public ActionResult GeneralGood(DutyCalculatorModel model)
        {
            ActionResult action;
            decimal? nullable;
            decimal? nullable1;
            try
            {
                string str = "%";
                HsCodeSlipter hsCodeSlipter = new HsCodeSlipter();
                hsCodeSlipter.Encode(model.generalform.HSCode);
                string str1 = hsCodeSlipter.Encode(model.generalform.HSCode);
               
                 var gethscode = paardb.HSCodeTariff.Where(x => x.HSCode == str1).ToList();
                if(gethscode==null)
                {
                    base.TempData["messageType"] = "danger";
                    base.TempData["message"] = "The HSCode that you entered is invalid. Please try again";
                    action = base.View(model);
                }
                else
                {
                    model.resultform = new GeneralGoodsResult();
                    hsCodeSlipter.Encode(model.generalform.HSCode);
                    var hSCodeTariff = (from r in this.paardb.HSCodeTariff  where r.HSCode == str1 select r).FirstOrDefault();
                    model.resultform.UnitOfQuality = hSCodeTariff.StandardUnitOfQuantity;
                    model.resultform.ImportDuty = string.Concat(hSCodeTariff.ImportDuty, str.ToString());
                    model.resultform.ImportNHIL = string.Concat(hSCodeTariff.ImportNHIL, str.ToString());
                    model.resultform.ImportVat = string.Concat(hSCodeTariff.ImportVAT, str.ToString());
                    model.resultform.GetFund = string.Concat(hSCodeTariff.GetFund, str.ToString());
                    model.resultform.ProcessingFee = string.Concat(hSCodeTariff.ProcessingFee, str.ToString());
                    model.resultform.CasseteLevy = string.Concat(hSCodeTariff.CasseteLevy, str.ToString());
                    model.resultform.ImportLevey = string.Concat(hSCodeTariff.ImportLevy, str.ToString());
                    model.resultform.EnivronmentalExcise = string.Concat(hSCodeTariff.EnvironmentalExcise, str.ToString());
                    model.resultform.AfricanUnion = string.Concat(hSCodeTariff.AfricanUnionImportlevy, str.ToString());
                    string str2 = hsCodeSlipter.Encode(model.generalform.HSCode).Substring(0, 4);
                    if ((
                        from x in this.paardb.TariffBenchmark
                        where x.HSCode == str2
                        select x).ToList<TariffBenchmark>().Any<TariffBenchmark>())
                    {
                        model.hasBechMakeValue = true;
                        base.TempData["messageType"] = "success";
                        base.TempData["message"] = "Dear valued customer/trader, your product may attract further depreciation in unit price after declaring  to GRA Customs for valuation and classification  assessment, this is due to the vice president directive of 50% reduction on commodity transactional value/unit price on the 4th of April 2019. Thank you <br><br> NB. Benchmark values are transaction values declared by importers and captured in our Tansaction Price Database (TPD); they are used as an internal risk assessment tool  for Customs valuation purposes by the Customs Technical Services Bureau. Benchmark values assist us to quickly assess valuation risks of targetted goods; we also use same to combat trade misinvoicing in the nature of underinvoicing, overinvoicing, valuation fraud and transfer pricing between related parties.";
                    }
                    var exchangeRate = (
                        from c in this.paardb.ExchangeRate
                        where c.CurrencyCode == model.generalform.CurrencyId
                        orderby c.Week descending
                        select c).FirstOrDefault();
                    GeneralGoodsResult generalGoodsResult = model.resultform;
                    decimal rate = exchangeRate.Rate;
                    generalGoodsResult.ExchangeRate = rate.ToString();
                    string str3 = " ";
                    decimal num = decimal.Parse(model.generalform.ProductValue);
                    model.resultform.FOB = string.Concat(exchangeRate.CurrencyCode, str3, num.ToString());
                    decimal num1 = decimal.Parse(model.generalform.Insurance);
                    decimal num2 = decimal.Parse(model.generalform.Freight);
                    decimal num3 = (num + num1) + num2;
                    model.resultform.CIF = string.Concat(exchangeRate.CurrencyCode, str3, num3.ToString());
                    int num4 = 100;
                    decimal num5 = decimal.Parse(hSCodeTariff.ImportDuty);
                    decimal num6 = (num3 * num5) / num4;
                    model.resultform.DutyPayable = string.Concat(exchangeRate.CurrencyCode, str3, num6.ToString("#,##0.00"));
                    decimal? importExcise = hSCodeTariff.ImportExcise;
                    decimal num7 = decimal.Parse(importExcise.ToString());
                    decimal getFund = hSCodeTariff.GetFund;
                    decimal num8 = ((num3 + num6) + num7) * (getFund / num4);
                    model.resultform.GetFund = string.Concat(exchangeRate.CurrencyCode, str3, num8.ToString("#,##0.00"));
                    decimal processingFee = (hSCodeTariff.ProcessingFee / num4) * num3;
                    model.resultform.ProcessingFee = string.Concat(exchangeRate.CurrencyCode, str3, processingFee.ToString("#,##0.00"));
                    decimal num9 = new decimal(5, 0, 0, false, 3) * num3;
                    model.resultform.EcoLevy = string.Concat(exchangeRate.CurrencyCode, str3, num9.ToString("#,##0.00"));
                    decimal casseteLevy = (hSCodeTariff.CasseteLevy / num4) * num3;
                    model.resultform.CasseteLevy = string.Concat(exchangeRate.CurrencyCode, str3, casseteLevy.ToString("#,##0.00"));
                    decimal num10 = new decimal(1, 0, 0, false, 2) * num3;
                    model.resultform.CustomsInspectionFee = string.Concat(exchangeRate.CurrencyCode, str3, num10.ToString("#,##0.00"));
                    decimal num11 = new decimal(1, 0, 0, false, 2) * num3;
                    model.resultform.IRSTaxDeposit = string.Concat(exchangeRate.CurrencyCode, str3, num11.ToString("#,##0.00"));
                    decimal importLevy = (hSCodeTariff.ImportLevy / num4) * num3;
                    model.resultform.ImportLevey = string.Concat(exchangeRate.CurrencyCode, str3, importLevy.ToString("#,##0.00"));
                    decimal environmentalExcise = (hSCodeTariff.EnvironmentalExcise / num4) * num3;
                    model.resultform.EnivronmentalExcise = string.Concat(exchangeRate.CurrencyCode, str3, environmentalExcise.ToString("#,##0.00"));
                    decimal num12 = new decimal(2, 0, 0, false, 2) * num3;
                    model.resultform.SIL = string.Concat(exchangeRate.CurrencyCode, str3, num12.ToString("#,##0.00"));
                    decimal num13 = new decimal(75, 0, 0, false, 4) * num3;
                    model.resultform.Exim = string.Concat(exchangeRate.CurrencyCode, str3, num13.ToString("#,##0.00"));
                    decimal? africanUnionImportlevy = hSCodeTariff.AfricanUnionImportlevy;
                    decimal num14 = num4;
                    if (africanUnionImportlevy.HasValue)
                    {
                        nullable = new decimal?(africanUnionImportlevy.GetValueOrDefault() / num14);
                    }
                    else
                    {
                        nullable = null;
                    }
                    importExcise = nullable;
                    rate = num3;
                    if (importExcise.HasValue)
                    {
                        nullable1 = new decimal?(importExcise.GetValueOrDefault() * rate);
                    }
                    else
                    {
                        africanUnionImportlevy = null;
                        nullable1 = africanUnionImportlevy;
                    }


                    decimal? nullable2 = nullable1;
                    if (nullable2 == null)
                    {
                        nullable2 = 0;
                    }

                     model.resultform.AfricanUnion = string.Concat(exchangeRate.CurrencyCode, str3, nullable2.ToString());
                    decimal num15 = decimal.Parse(hSCodeTariff.ImportNHIL);
                    decimal num16 = ((num3 + num6) + num7) * (num15 / num4);
                    model.resultform.NHILPayble = string.Concat(exchangeRate.CurrencyCode, str3, num16.ToString("#,##0.00"));
                    decimal num17 = decimal.Parse(hSCodeTariff.ImportVAT);
                    decimal num18 = num3 + num6 + num7 + num8 + num16 + decimal.Parse(nullable2.ToString()) + num13 + num12 + environmentalExcise + importLevy + num11 + num10 + casseteLevy + num9 + processingFee + num8 * (num17 / num4);
                    model.resultform.VatPayble = string.Concat(exchangeRate.CurrencyCode, str3, num18.ToString("#,##0.00"));
                    decimal num19 = (num6 + num18) + num16;
                    GeneralGoodsResult generalGoodsResult1 = model.resultform;
                    string currencyCode = exchangeRate.CurrencyCode;
                    rate = Math.Round(num19, 2);
                    generalGoodsResult1.TotalDuty = string.Concat(currencyCode, str3, rate.ToString("#,##0.00"));
                    string str4 = "GHC";
                    rate = exchangeRate.Rate;
                    decimal num20 = decimal.Parse(rate.ToString("#,##0.00"));
                    decimal num21 = Math.Round(num19 * num20, 2);
                    model.resultform.TotalDutyPayable = string.Concat(str4, str3, num21.ToString("#,##0.00"));
                    model.CurrencyList = new SelectList((
                        from x in this.paardb.ExchangeRateView
                        orderby x.Week descending
                        select x).Take<ExchangeRateView>(18), "CurrencyCode", "CurrencyName");
                    DutyCounter totalUsed = (
                        from x in this.db.DutyCounter
                        where x.CounterType == "General Goods"
                        select x).FirstOrDefault<DutyCounter>();
                    totalUsed.TotalUsed = totalUsed.TotalUsed + 1;
                    action = base.View(model);
                }
            }
            catch (Exception exception1)
            {
                Exception exception = exception1;
                base.TempData["messageType"] = "danger";
                base.TempData["message"] = "An Error occur. Please try again later or contact the system administrator";
                ErrorSignal.FromCurrentContext().Raise(exception);
                action = base.RedirectToAction("GeneralGood");
            }
            return action;
        }




        public JsonResult GetMakeType()
        {
            return Json(swdb.VehicleReference.ToList(), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetModelByMakeId(string makeId)
        {
            int Id = Convert.ToInt32(makeId);


            var models = from a in swdb.VehicleModelReference where a.MakeId == Id select a;

            return Json(models);
        }


        [HttpPost]
        public ActionResult AddFeedback(DutyCalculatorModel model)
        {          
            if (ModelState.IsValid)
            {
                try
                {
                    HandlingService handlingService = new HandlingService();
                    var alert = (from x in chasmdb.Alert where x.Id == 7043 select x).FirstOrDefault();

                    var alert1 = (from x in this.chasmdb.Alert where x.Id == 7044  select x).FirstOrDefault();

                    ContactUs addnew = new ContactUs
                    {
                        Fullname = model.contactusform.fullname,
                        EmailAddress = model.contactusform.EmailAddress,
                        PhoneNo = model.contactusform.MobileNumber,
                        Message = model.contactusform.Message,
                        SentDate = DateTime.Now,
                        MessageType = "Feedback",
                        IsReply = false
                    };
                    db.ContactUs.AddObject(addnew);
                    db.SaveChanges();
                    handlingService.SendEmailNotificationToUser(alert.SubjectEmail, alert.Email.Replace("%FullName%", model.contactusform.fullname), model.contactusform.EmailAddress, "no-reply@ghanatradinghub.com", alert.Id);
                   // handlingService.SendSMSNotificationToUser(alert.SubjectSms, alert.Sms.Replace("%FullName%", model.contactusform.fullname), model.contactusform.MobileNumber, alert.SubjectSms, alert.Id);
                    List<Users> users = new List<Users>();
                    var rl = swdb.Roles.SingleOrDefault(x => x.RoleName == "GTH Admin");

                    var GetUser = rl.Users.ToList();

                    foreach (var list in GetUser)
                    {
                        var userDetail = (from x in this.swdb.UserDetails where x.UserId == list.UserId select x).FirstOrDefault();
                        //if (userDetail != null && userDetail.PhoneNumber != null)
                        //{
                        //    handlingService.SendSMSNotificationToAdmin(alert1.SubjectSms, alert1.Sms.Replace("%Username%", userDetail.FirstName), userDetail.PhoneNumber, alert1.SubjectSms, alert1.Id);
                        //}
                        handlingService.SendEmailNotificationToChamsAdmin(alert1.Title, alert1.Email.Replace("%Username%", list.UserName), list.Memberships.Email, model.contactusform.EmailAddress, alert1.Id);
                    }

                    TempData["MessageType"] = "success";
                    TempData["Message"] = "Your Message has been sent successully. We'll review your message and contact you. <b>Thank You.</b>";               
                    return RedirectToAction("UsedVehicle");
                }
                catch(Exception ex)
                {
                    TempData["messageType"] = "danger";
                    TempData["message"] = "An Error occur. Please try again later or contact the system administrator";
                    Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                    return RedirectToAction("UsedVehicle");

                }

            }
            else
            {
                 TempData["messageType"] = "danger";
                TempData["message"] = "Please provide required fields value.";
                return View(model);
            }


            //if (Request.IsAjaxRequest())
            //{
            //    return new JsonResult { Data = message, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            //}
            //else
            //{
            //    ViewBag.Message = message;
            //    return View(model);
            //}
        }
   


        [HttpPost]
        public ActionResult GetModelNames(int mId)
        {           
            List<SelectListItem> modelNames = new List<SelectListItem>();           
            List<sw.VehicleModelReference> districts = swdb.VehicleModelReference.Where(x => x.MakeId == mId).ToList();
            districts.ForEach(x =>
            {
                modelNames.Add(new SelectListItem { Text = x.ModelName, Value = x.Id.ToString() });
            });
            return Json(modelNames, JsonRequestBehavior.AllowGet);
        }

        public ActionResult UsedVehicle()
        {
            DutyCalculatorModel model = new DutyCalculatorModel();       
            List<SelectListItem> makeNames = new List<SelectListItem>();         
            List<sw.VehicleReference> makes = swdb.VehicleReference.OrderBy(x=>x.Name).ToList();
            makes.ForEach(x =>
            {
                makeNames.Add(new SelectListItem { Text = x.Name, Value = x.Id.ToString() });
            });
            model.MakeNames = makeNames;
            return View(model);
        }

        [HttpPost]
        public ActionResult UsedVehicle(DutyCalculatorModel model)
        {
            try
            {
                var GetMakeName = swdb.VehicleReference.Where(x => x.Id == model.VehicleTypeform.MakeId).FirstOrDefault();
                var GetModelName = swdb.VehicleModelReference.Where(x => x.Id == model.VehicleTypeform.ModelId).FirstOrDefault();
                string vy = model.VehicleTypeform.Year;
                var GetVehicleDetails1 = (from v in paardb.ValuationTPD where v.Make == GetMakeName.Name && v.Model == GetModelName.ModelName && v.ManufactureYear == vy select v).Distinct().ToList();
                if (GetVehicleDetails1.Any())
                {
                    var Getv1 = (from v in paardb.ValuationTPD where v.Make == GetMakeName.Name && v.Model == GetModelName.ModelName && v.ManufactureYear == vy select v).Take(1).ToList();
                    model.VehicleList = Getv1;
                }
                else
                {
                    string[] nameParts = GetModelName.ModelName.Split(' ');
                    foreach (var i in nameParts)
                    {
                        string vm = i;
                        var GetVehicleDetails2 = (from v in paardb.ValuationTPD where v.Make == GetMakeName.Name && v.ManufactureYear == vy && (v.Model.Contains(vm)) select v).Distinct().ToList();
                        if (GetVehicleDetails2.Any())
                        {
                            var Getv2 = (from v in paardb.ValuationTPD where v.Make == GetMakeName.Name && v.ManufactureYear == vy && (v.Model.Contains(vm)) select v).Distinct().Take(2).ToList();
                            model.VehicleList = Getv2;
                        }
                    }



                }
             //   model.VehicleTypeMake = (from s in swdb.VehicleReference where s.Id == GetMakeName.Id select new IntegerSelectListItem { Text = s.Name, Value = s.Id }).ToList();
              //  model.VehicleModel = (from s in swdb.VehicleModelReference where s.Id == GetModelName.Id select new IntegerSelectListItem { Text = s.ModelName, Value = s.Id }).ToList();

                return View(model);
                // return RedirectToAction("UsedVehicleResult",new {VehicleMake=GetMakeName.Name, VehicleModel=GetModelName.ModelName, VehicleYear=model.VehicleTypeform.Year});
            }
            catch (Exception ex)
            {
                TempData["messageType"] = "danger";
                TempData["message"] = "An Error occur. Please try again later or contact the system administrator";
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                return RedirectToAction("Index");
            }
        }


        public ActionResult GetResult(int VehicleMake, int VehicleModel, string VehicleYear)
        {
            try
            {
                DutyCalculatorModel model = new DutyCalculatorModel();
                var GetMakeName = GetMake(VehicleMake);
                var GetModelName = GetModel(VehicleModel, VehicleMake);
                model.VehicleList = GetValidVehicles(paardb, GetMakeName, GetModelName, VehicleYear);

                //Adding Number of time the tool has been used
                var GetCounter = db.DutyCounter.Where(x => x.CounterType == "Used Vehicle").FirstOrDefault();
                int oldCounter = GetCounter.TotalUsed;
                int addCount = 1;
                int totalCounter = oldCounter + addCount;
                GetCounter.TotalUsed = totalCounter;
                db.SaveChanges();



                //check for related Year
                // removing 1 year from the year entered
                int gyear = int.Parse(VehicleYear);
                int YearRemoved = RemoveYear(gyear);
                string YearMinu = YearRemoved.ToString();

                //adding one year to the year entered
                int YearEntered = int.Parse(VehicleYear);
                int YearAdded = AddYear(YearEntered);
                string YearGotten = YearAdded.ToString();

                //ValuationTPD Vehicle List ------Search initialization---------
                if (model.VehicleList.Any())
                {
                    //100% record match for search. So display result here
                    var MatchRecord = (from v in paardb.ValuationTPD where v.Make == GetMakeName && v.Model == GetModelName && v.ManufactureYear == VehicleYear select v).OrderByDescending(x => x.HDV).Take(1).ToList();
                    model.VehicleList = MatchRecord;
                }
                else
                {
                    //Connot find the year entered but able to find previous year. so it display the result of previous years cars
                    var MatchRecordModel1 = (from v in paardb.ValuationTPD where v.Make == GetMakeName && v.Model == GetModelName && (v.ManufactureYear.Contains(YearMinu) || v.ManufactureYear.Contains(YearGotten)) select v).OrderByDescending(x => x.HDV).Take(2).ToList();
                    if(MatchRecordModel1.Any())
                    {
                        TempData["messageType"] = "success";
                        TempData["message"] = "We cannot find the HDV for " + VehicleYear + ". below are related search result for " + YearMinu + " or "+ YearGotten + " vehicles";
                        model.VehicleList = MatchRecordModel1;
                    }
                    else
                    {
                        //Connot find the year entered but able to add one more year. so it display the result of years enter plus one year
                        var MatchRecordModel2 = (from v in paardb.ValuationTPD where v.Make == GetMakeName && v.Model.Contains(GetModelName) && (v.ManufactureYear.Contains(YearMinu) || v.ManufactureYear.Contains(YearGotten) || v.ManufactureYear == VehicleYear) select v).OrderByDescending(x => x.HDV).Take(2).ToList();
                        if(MatchRecordModel2.Any())
                        {
                            TempData["messageType"] = "success";
                            TempData["message"] = "We cannot find the HDV for " + VehicleYear + ". below are related search result for " + GetModelName + " vehicles";
                            model.VehicleList = MatchRecordModel2;
                        }
                        else
                        {
                            //check for model entered. find record so, split model name and loop through. Eg Honda Accord.
                            string[] nameParts = GetModelName.Split(' ');
                            foreach (var i in nameParts)
                            {
                                string vm = i;
                                var SliptedRecordFound = (from v in paardb.ValuationTPD where v.Make == GetMakeName && v.ManufactureYear == VehicleYear && v.Model.Contains(vm) select v).OrderByDescending(x => x.HDV).Take(1).Distinct().ToList();
                                if (SliptedRecordFound.Any())
                                {
                                    TempData["messageType"] = "success";
                                    TempData["message"] = "We cannot find HDV for " + GetModelName + " vehicle. below are related search result for " + VehicleYear + " vehicles";
                                    model.VehicleList = SliptedRecordFound;
                                    break;
                                }
                                else
                                {
                                    var SliptedRecordNotFound = (from v in paardb.ValuationTPD where v.Make == GetMakeName && v.Model.Contains(vm) && (v.ManufactureYear.Contains(YearMinu)) select v).OrderByDescending(x => x.HDV).Take(1).Distinct().ToList();
                                    if (SliptedRecordNotFound.Any())
                                    {
                                        TempData["messageType"] = "success";
                                        TempData["message"] = "We cannot find HDV for " + VehicleYear + " vehicle. below are related search result for " + YearMinu + " vehicles";
                                        model.VehicleList = SliptedRecordNotFound;
                                        break;
                                    }
                                    else
                                    {
                                        var SliptedRecordNotFound2 = (from v in paardb.ValuationTPD where v.Make == GetMakeName && v.Model.Contains(vm) && (v.ManufactureYear.Contains(YearGotten)) select v).OrderByDescending(x => x.HDV).Take(2).Distinct().ToList();
                                        if (SliptedRecordNotFound2.Any())
                                        {
                                            TempData["messageType"] = "success";
                                            TempData["message"] = "We cannot find HDV for " + VehicleYear + " vehicle. below are related search result for " + YearGotten + " vehicles";
                                            model.VehicleList = SliptedRecordNotFound2;
                                            break;
                                        }
                                        else
                                        {
                                            #region optional search begins here

                                            //check for year and make
                                            var CheckMakeYear = (from v in paardb.ValuationTPD where v.Make == GetMakeName && v.ManufactureYear == VehicleYear select v).OrderByDescending(x => x.HDV).Take(3).Distinct().ToList();
                                            if (CheckMakeYear.Any())
                                            {
                                                TempData["messageType"] = "success";
                                                TempData["message"] = "We cannot find the HDV for " + GetModelName + " vehicle. below are related search result for " + VehicleYear + " vehicles";
                                                model.VehicleList = CheckMakeYear;
                                                break;
                                            }
                                            else
                                            {
                                                var CheckMakeRelatedYear = (from v in paardb.ValuationTPD where v.Make == GetMakeName && v.ManufactureYear.Contains(YearMinu) select v).OrderByDescending(x => x.HDV).Take(3).Distinct().ToList();
                                                if (CheckMakeRelatedYear.Any())
                                                {
                                                    TempData["messageType"] = "success";
                                                    TempData["message"] = "We cannot find the HDV for " + GetModelName + " vehicle. below are related search result between " + YearMinu + " vehicles";
                                                    model.VehicleList = CheckMakeRelatedYear;
                                                    break;
                                                }
                                            }

                                            var CheckMakeOnly = (from v in paardb.ValuationTPD where v.Make == GetMakeName && (v.ManufactureYear.Contains(YearGotten)) select v).OrderByDescending(x => x.HDV).Take(3).Distinct().ToList();
                                            if (CheckMakeOnly.Any())
                                            {
                                                TempData["messageType"] = "success";
                                                TempData["message"] = "We cannot find the HDV for " + GetModelName + " " + VehicleYear + " HDV. below are related search result for " + YearGotten + " " + GetMakeName + " vehicles";
                                                model.VehicleList = CheckMakeOnly;
                                                break;
                                            }
                                            else
                                            {
                                                var CheckRelatedMake = (from v in paardb.ValuationTPD where v.Make.Contains(GetMakeName) select v).OrderByDescending(x => x.HDV).Take(3).Distinct().ToList();
                                                if (CheckRelatedMake.Any())
                                                {
                                                    TempData["messageType"] = "success";
                                                    TempData["message"] = "We cannot find the HDV for " + GetModelName + " " + VehicleYear + " HDV. below are related search result for " + GetMakeName + " vehicles";
                                                    model.VehicleList = CheckRelatedMake;
                                                    break;
                                                }
                                            }


                                            #endregion optional serch end here
                                        }
                                    }

                                }
                            }
                        }
                    }
                }                       
                return PartialView("SearchResult", model);
            }
            catch (Exception ex)
            {
                TempData["messageType"] = "danger";
                TempData["message"] = "An Error occur. Please try again later or contact the system administrator";
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                return null;
            }

        }


        public ActionResult GetChassis(string chassis)
        {
            try
            {
                DutyCalculatorModel model = new DutyCalculatorModel();
                model.VehicleList = GetChassis(paardb, chassis);   
                
                if(model.VehicleList.Any())
                {
                    //Adding Number of time the tool has been used
                    var GetCounter = db.DutyCounter.Where(x => x.CounterType == "Used Vehicle").FirstOrDefault();
                    int oldCounter = GetCounter.TotalUsed;
                    int addCount = 1;
                    int totalCounter = oldCounter + addCount;
                    GetCounter.TotalUsed = totalCounter;
                    db.SaveChanges();

                    model.VehicleList = GetChassis(paardb, chassis);
                    return PartialView("SearchResultChassis", model);
                }
                string eightword = chassis.Substring(0, 8);
                List<ValuationTPD> eightTPD = paardb.ValuationTPD.Where(x => x.ChassisNo.StartsWith(eightword)).Take(10).Distinct().ToList();
                if (eightTPD.Any())
                {
                    //Adding Number of time the tool has been used
                    var GetCounter = db.DutyCounter.Where(x => x.CounterType == "Used Vehicle").FirstOrDefault();
                    int oldCounter = GetCounter.TotalUsed;
                    int addCount = 1;
                    int totalCounter = oldCounter + addCount;
                    GetCounter.TotalUsed = totalCounter;
                    db.SaveChanges();

                    TempData["messageType"] = "success";
                    TempData["message"] = "We cannot decode your chassis no. but below are similar chassis no that matches your chassis "+eightword+" ";                   
                    model.VehicleList = eightTPD;
                }
                else
                {
                    string fourword = chassis.Substring(0, 4);
                    List<ValuationTPD> fourTPD = paardb.ValuationTPD.Where(x => x.ChassisNo.StartsWith(fourword)).Take(10).Distinct().ToList();
                    if (fourTPD.Any())
                    {
                        //Adding Number of time the tool has been used
                        var GetCounter = db.DutyCounter.Where(x => x.CounterType == "Used Vehicle").FirstOrDefault();
                        int oldCounter = GetCounter.TotalUsed;
                        int addCount = 1;
                        int totalCounter = oldCounter + addCount;
                        GetCounter.TotalUsed = totalCounter;
                        db.SaveChanges();

                        TempData["messageType"] = "success";
                        TempData["message"] = "We cannot decode your chassis no. but below are similar chassis no that matches your chassis " + fourword + " ";
                        model.VehicleList = fourTPD;
                    }
                    else
                    {
                        TempData["messageType"] = "danger";
                        TempData["message"] = "We cannot decode you chassis no. Please try again chassis no";
                    }
                }            
                return PartialView("SearchResultChassis", model);
            }
            catch(Exception ex)
            {
                TempData["messageType"] = "danger";
                TempData["message"] = "An Error occur. Please try again later or contact the system administrator";
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                return null;
            }
        }
        //public ActionResult ViewDutySample(int Id)
        //{
        //    try
        //    {
        //        DutyCalculatorModel model = new DutyCalculatorModel();
        //        var GetTDP = paardb.ValuationTPD.Where(x => x.ID == Id).FirstOrDefault();
        //        decimal CC = decimal.Parse(GetTDP.EngineCapacity.ToString());

        //        #region other methods used               



        //        var GetCurrency = (from c in paardb.ExchangeRate where c.CurrencyCode == GetTDP.Currency select c).OrderByDescending(c => c.Week).FirstOrDefault();
        //        var GetHSCode = (from c in swdb.br_HSCodeTariff where c.HSCode == GetTDP.AssessedHSCode select c).FirstOrDefault();

        //        HSCodes hSCode = (from c in this.dbclass.HSCodes where c.Code ==GetTDP.AssessedHSCode select c).FirstOrDefault<HSCodes>();

        //        if (GetHSCode == null)
        //        {
        //            TempData["messageType"] = "danger";
        //            TempData["message"] = "Cannot find the hscode of ValuationTPD in HSCode table (Collaboration)";
        //            //  Elmah.ErrorSignal.FromCurrentContext().Raise("Cannot find the hscode of ValuationTPD in HSCode table (Collaboration)");
        //            return RedirectToAction("Index");
        //        }
        //        sw.v_FreightOverage fvalue = new sw.v_FreightOverage();

        //        string space = " ";
        //        var curMonth = DateTime.Now.Month;
        //        string half = "First";
        //        if (curMonth > 6)
        //            half = "Second";
        //        model.UsedVehicleDutyResultForm = new UsedVehicleDutyResultForm();
        //        model.UsedVehicleDutyResultForm.rate = GetCurrency.Rate.ToString();
        //        model.UsedVehicleDutyResultForm.shppers = "5.00";

        //        int age = DateTime.Now.Year - int.Parse(GetTDP.ManufactureYear);


        //        var GetVehicleType = swdb.sid_VehicleTypes.Where(x => x.TypeName == GetTDP.VehicleType).FirstOrDefault();
        //        if (GetVehicleType == null)
        //        {

        //        }
        //        var GetOverAge = swdb.v_FreightOverage.Where(x => x.typename == GetTDP.VehicleType && age >= x.MinimumAge && age <= x.MaximumAge).FirstOrDefault();
        //        var freightreates = this.swdb.sid_FreightRates.Where(x => x.VehicleCategoryId == GetVehicleType.VehicleCategoryId).ToList();
        //        if (freightreates.Count > 1)
        //        {
        //            freightreates = freightreates.Where(x => x.VehicleCategoryId == GetVehicleType.VehicleCategoryId && x.MinimumCC <= CC && x.MaximumCC >= CC).ToList();
        //        }

        //        sw.sid_FreightRates row = new sw.sid_FreightRates();
        //        if (freightreates.Count >= 1)
        //        {
        //            row = freightreates.FirstOrDefault();
        //        }

        //        model.UsedVehicleDutyResultForm.freight = row.FreightRate.ToString();
        //        //fvalue.freightrate.ToString();

        //        DepreciationClass newclass = new DepreciationClass();
        //        var resp = newclass.CalDepreciation(int.Parse(GetTDP.ManufactureYear), decimal.Parse(GetTDP.HDV.ToString()), half);
        //        model.UsedVehicleDutyResultForm.depreciation = resp.ToString();

        //        #endregion


        //        int num = 100;
        //        #region /////////////////////FOB Calculation  start here///////////
        //        decimal TotalFOB;
        //        decimal hdv = decimal.Parse(GetTDP.HDV.ToString());
        //        TotalFOB = hdv - resp;
        //        model.UsedVehicleDutyResultForm.FOB = TotalFOB;
        //        #endregion// //////////////////FOB Calculation ends here///////////////////

        //        #region /////////////////////Insurance Calculation  start here///////////
        //        decimal TotalInsurance;
        //        double ivalues = 0.875;
        //        TotalInsurance = (decimal.Parse(TotalFOB.ToString()) + decimal.Parse(row.FreightRate.ToString())) * (decimal.Parse(ivalues.ToString()) / num);
        //        model.UsedVehicleDutyResultForm.insurance = decimal.Parse(TotalInsurance.ToString("N"));
        //        #endregion ////////////////////Insurance Calculation  end here///////////

        //        #region ////////////////////CIF Calculation start here/////////////
        //        decimal TotalCIF;
        //        decimal InsuranceValue = decimal.Parse(TotalInsurance.ToString());
        //        decimal FrightValue = decimal.Parse(row.FreightRate.ToString());
        //        TotalCIF = TotalFOB + InsuranceValue + FrightValue;
        //        model.UsedVehicleDutyResultForm.cif = TotalCIF.ToString("N") + space + GetCurrency.CurrencyCode;
        //        #endregion ////////////////////CIF Calculation end here/////////////

        //        #region ////////////////////CIF Calculation start here in Local Currency/////////////
        //        string LocalCurrency = "CEDI";
        //        decimal TotalCIFLocal;
        //        decimal ExRate = decimal.Parse(GetCurrency.Rate.ToString());
        //        TotalCIFLocal = TotalCIF * ExRate;
        //        decimal Val = Math.Round(TotalCIFLocal, 2);
        //        model.UsedVehicleDutyResultForm.ghcedis = LocalCurrency + space + Val.ToString("N");
        //        #endregion ////////////////////CIF Calculation end here in Local Currency/////////////

        //        #region////////////////////Import Duty Calculation start here /////////////
        //        decimal TotalImportDuty;

        //        decimal ImportDutyValue = decimal.Parse(GetHSCode.ImportDuty.ToString());
        //        TotalImportDuty = (decimal.Parse(ImportDutyValue.ToString()) / num) * TotalCIFLocal;
        //        model.UsedVehicleDutyResultForm.Importduty = TotalImportDuty.ToString("N");
        //        #endregion ////////////////////Import Duty Calculation end here /////////////

        //        #region ////////////////////Processing Fee Calculation start here /////////////
        //        decimal TotalProcessingFee;
        //        string ProcessingFeeValue = "0.01";
        //        if (TotalImportDuty > 0)
        //        {

        //            TotalProcessingFee = 0;
        //            model.UsedVehicleDutyResultForm.ProcessingFee = TotalProcessingFee.ToString("N");
        //        }
        //        else
        //        {


        //            TotalProcessingFee = (TotalCIFLocal * decimal.Parse(ProcessingFeeValue));
        //            model.UsedVehicleDutyResultForm.ProcessingFee = TotalProcessingFee.ToString("N");
        //        }

        //        #endregion ////////////////////Processing Fee Calculation end here /////////////

        //        #region   ////////////////////Vat Calculation start here /////////////
        //        decimal TotalVat;
        //        decimal vatRate = 15;
        //        TotalVat = (vatRate / num) * (TotalCIFLocal + TotalImportDuty);
        //        model.UsedVehicleDutyResultForm.vat = TotalVat.ToString("N");
        //        #endregion ////////////////////Vat Calculation end here /////////////

        //        #region  ////////////////////NHIL Calculation start here /////////////
        //        decimal TotalNHIL;
        //        TotalNHIL = (decimal.Parse(GetHSCode.NHIL.ToString()) / num) * (TotalCIFLocal + TotalImportDuty);
        //        model.UsedVehicleDutyResultForm.nhil = TotalNHIL.ToString("N");
        //        #endregion  ////////////////////NHIL Calculation end here /////////////

        //        #region  ////////////////////Interest charge Calculation start here /////////////
        //        decimal TotalInterestCharge;
        //        int SNum = 48;
        //        TotalInterestCharge = (TotalImportDuty + TotalVat) / SNum;
        //        model.UsedVehicleDutyResultForm.interestCharge = TotalInterestCharge.ToString("N");
        //        #endregion  ////////////////////Interest charge Calculation end here /////////////

        //        #region  ////////////////////ECowas rate Calculation start here /////////////
        //        decimal TotalECowasRate;
        //        string eco = "0.75";
        //        TotalECowasRate = (TotalCIFLocal * decimal.Parse(eco)) / num;
        //        model.UsedVehicleDutyResultForm.ecowas = TotalECowasRate.ToString("N");
        //        #endregion  ////////////////////ECowas rate  Calculation end here /////////////

        //        #region  ////////////////////Network charge Calculation start here /////////////
        //        decimal TotalNetworkCharge;
        //        string net = "0.4";
        //        decimal GetFob = decimal.Parse(TotalFOB.ToString()) * decimal.Parse(GetCurrency.Rate.ToString());
        //        TotalNetworkCharge = (decimal.Parse(net) / num) * GetFob;
        //        model.UsedVehicleDutyResultForm.networkCharge = TotalNetworkCharge.ToString("N");
        //        #endregion  ////////////////////Network charge  Calculation end here /////////////

        //        #region  ////////////////////EDIF Payble Calculation start here /////////////
        //        decimal TotalEDIF;
        //        TotalEDIF = (TotalCIFLocal * decimal.Parse(eco)) / num;
        //        model.UsedVehicleDutyResultForm.edif = TotalEDIF.ToString("N");
        //        #endregion  ////////////////////EDIF Payble  Calculation end here /////////////

        //        #region  ////////////////////Net Charge Vat Calculation start here /////////////
        //        decimal TotalNetchargeVat;
        //        decimal netVat = 15;
        //        TotalNetchargeVat = TotalNetworkCharge * (netVat / num);
        //        model.UsedVehicleDutyResultForm.NetchargeVat = TotalNetchargeVat.ToString("N");
        //        #endregion  ////////////////////Net Charge Vat  Calculation end here /////////////



        //        #region  Exam Fee Calculations
        //        decimal TotalExamFee;
        //        TotalExamFee = (TotalCIFLocal * decimal.Parse(ProcessingFeeValue));
        //        model.UsedVehicleDutyResultForm.examfee = TotalExamFee.ToString("N");
        //        #endregion


        //        #region  NetCharge NHIL Calculations
        //        decimal TotalNetChargeNHIL;
        //        string FeeValue = "2.5";
        //        TotalNetChargeNHIL = TotalNetworkCharge * (decimal.Parse(FeeValue) / num);
        //        model.UsedVehicleDutyResultForm.NetchargeNhil = TotalNetChargeNHIL.ToString("N");
        //        #endregion

        //        #region Levy Calculations
        //        decimal TotalLevy;
        //        string AllKind = "2.0";
        //        TotalLevy = TotalCIFLocal * (decimal.Parse(AllKind) / num);
        //        model.UsedVehicleDutyResultForm.specialLevy = TotalLevy.ToString("N");


        //        #endregion


        //        #region Overage Calaclations
        //        decimal TotalOverAge;
        //        TotalOverAge = TotalCIFLocal * (decimal.Parse(GetOverAge.OverAgeRate.ToString()) / num);
        //        model.UsedVehicleDutyResultForm.Overage = TotalOverAge.ToString("N");
        //        #endregion


        //        #region Transaction Currency Calculations
        //        decimal TotalTransactionCurrency;
        //        decimal shipperFee = 5;

        //        TotalTransactionCurrency = TotalImportDuty + TotalVat + TotalNHIL + TotalECowasRate + TotalEDIF + TotalOverAge + TotalExamFee + TotalInterestCharge + TotalLevy + TotalProcessingFee + TotalNetworkCharge + TotalNetchargeVat + TotalNetChargeNHIL + shipperFee;
        //        model.UsedVehicleDutyResultForm.localCurrency = TotalTransactionCurrency.ToString("N");

        //        #endregion
        //        model.ValuationTPD = GetTDP;
        //        return View(model);
        //    }
        //    catch (Exception ex)
        //    {
        //        TempData["messageType"] = "danger";
        //        TempData["message"] = "An Error occur. Please try again later or contact the system administrator";
        //        Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
        //        return RedirectToAction("GeneralGood");
        //    }
        //}


        public ActionResult ViewDuty(int Id)
        {
            decimal num;
            decimal num1;
            ActionResult action;
            try
            {
                DutyCalculatorModel dutyCalculatorModel = new DutyCalculatorModel();
                ValuationTPD valuationTPD = (
                    from x in this.paardb.ValuationTPD
                    where x.ID == Id
                    select x).FirstOrDefault<ValuationTPD>();
                decimal? hDV = valuationTPD.HDV;
                decimal num2 = decimal.Parse(hDV.ToString());
                double num3 = 0.3;
                decimal num4 = num2 - (num2 * decimal.Parse(num3.ToString()));
                decimal num5 = decimal.Parse(valuationTPD.EngineCapacity.ToString());
                var exchangeRate = (from c in this.paardb.ExchangeRate where c.CurrencyCode == valuationTPD.Currency orderby c.Week descending select c).FirstOrDefault();
                var hSCode = (from c in swdb.br_HSCodeTariff where c.HSCode == valuationTPD.AssessedHSCode select c).FirstOrDefault();
                if (hSCode != null)
                {
                    v_FreightOverage vFreightOverage = new v_FreightOverage();
                    string str = " ";
                    DateTime now = DateTime.Now;
                    string str1 = "First";
                    if (now.Month > 6)
                    {
                        str1 = "Second";
                    }
                    dutyCalculatorModel.UsedVehicleDutyResultForm = new UsedVehicleDutyResultForm()
                    {
                        rate = exchangeRate.Rate.ToString(),
                        shppers = "5.00"
                    };
                    now = DateTime.Now;
                    int year = now.Year - int.Parse(valuationTPD.ManufactureYear);
                    var sidVehicleType = (from x in this.paardb.sid_VehicleTypes where x.TypeName == valuationTPD.VehicleType  select x).FirstOrDefault();
                    var vFreightOverage1 = (from x in this.dbclass.v_FreightOverage where x.typename == valuationTPD.VehicleType && (int?)year >= x.MinimumAge && (int?)year <= x.MaximumAge select x).FirstOrDefault();
                    var list = (from x in this.paardb.sid_FreightRates  where (int?)x.VehicleCategoryId == sidVehicleType.VehicleCategoryId  select x).ToList();
                    if (list.Count > 1)
                    {
                        list = list.Where<sid_FreightRates>((sid_FreightRates x) => {
                            decimal? nullable;
                            decimal? nullable1;
                            decimal? nullable2;
                            int vehicleCategoryId = x.VehicleCategoryId;
                            int? minimumCC = sidVehicleType.VehicleCategoryId;
                            if ((vehicleCategoryId == minimumCC.GetValueOrDefault() ? minimumCC.HasValue : false))
                            {
                                minimumCC = x.MinimumCC;
                                if (minimumCC.HasValue)
                                {
                                    nullable1 = new decimal?(minimumCC.GetValueOrDefault());
                                }
                                else
                                {
                                    nullable = null;
                                    nullable1 = nullable;
                                }
                                decimal? nullable3 = nullable1;
                                decimal cC = num5;
                                if ((nullable3.GetValueOrDefault() <= cC ? nullable3.HasValue : false))
                                {
                                    minimumCC = x.MaximumCC;
                                    if (minimumCC.HasValue)
                                    {
                                        nullable2 = new decimal?(minimumCC.GetValueOrDefault());
                                    }
                                    else
                                    {
                                        nullable = null;
                                        nullable2 = nullable;
                                    }
                                    nullable3 = nullable2;
                                    cC = num5;
                                    if (nullable3.GetValueOrDefault() < cC)
                                    {
                                        return false;
                                    }
                                    return nullable3.HasValue;
                                }
                            }
                            return false;
                        }).ToList<sid_FreightRates>();
                    }
                    sid_FreightRates sidFreightRate = new sid_FreightRates();
                    if (list.Count >= 1)
                    {
                        sidFreightRate = list.FirstOrDefault<sid_FreightRates>();
                    }
                    dutyCalculatorModel.UsedVehicleDutyResultForm.freight = sidFreightRate.FreightRate.ToString();
                    int num6 = 100;
                    DepreciationClass depreciationClass = new DepreciationClass();
                    int num7 = int.Parse(valuationTPD.ManufactureYear);
                    hDV = valuationTPD.HDV;
                    decimal num8 = depreciationClass.CalDepreciation(num7, decimal.Parse(hDV.ToString()), str1);
                    dutyCalculatorModel.UsedVehicleDutyResultForm.depreciation = num8.ToString();
                    decimal num9 = depreciationClass.CalDepreciation(int.Parse(valuationTPD.ManufactureYear), num4, str1);
                    dutyCalculatorModel.UsedVehicleDutyResultForm.depreciationafterdedution = num9.ToString();
                    decimal num10 = decimal.Parse(num4.ToString()) - num9;
                    dutyCalculatorModel.UsedVehicleDutyResultForm.FOBafterdedution = num10;
                    double num11 = 0.875;
                    decimal num12 = decimal.Parse(num10.ToString());
                    hDV = sidFreightRate.FreightRate;
                    decimal num13 = (num12 + decimal.Parse(hDV.ToString())) * (decimal.Parse(num11.ToString()) / num6);
                    dutyCalculatorModel.UsedVehicleDutyResultForm.insuranceafterdedution = decimal.Parse(num13.ToString("N"));
                    decimal num14 = decimal.Parse(num13.ToString());
                    hDV = sidFreightRate.FreightRate;
                    decimal num15 = decimal.Parse(hDV.ToString());
                    decimal num16 = (num10 + num14) + num15;
                    dutyCalculatorModel.UsedVehicleDutyResultForm.cifafterdedution = string.Concat(num16.ToString("N"), str, exchangeRate.CurrencyCode);
                    string str2 = "CEDI";
                    decimal rate = exchangeRate.Rate;
                    decimal num17 = decimal.Parse(rate.ToString());
                    decimal num18 = num16 * num17;
                    decimal num19 = Math.Round(num18, 2);
                    dutyCalculatorModel.UsedVehicleDutyResultForm.ghcedisafterdedution = string.Concat(str2, str, num19.ToString("N"));
                    hDV = decimal.Parse(hSCode.ImportDuty);
                    decimal num20 = decimal.Parse(hDV.ToString());
                    decimal num21 = (decimal.Parse(num20.ToString()) / num6) * num18;
                    dutyCalculatorModel.UsedVehicleDutyResultForm.Importdutyafterdedution = num21.ToString("N");
                    string str3 = "0.01";
                    if (num21 <= decimal.Zero)
                    {
                        num = num18 * decimal.Parse(str3);
                        dutyCalculatorModel.UsedVehicleDutyResultForm.ProcessingFeeafterdedution = num.ToString("N");
                    }
                    else
                    {
                        num = new decimal();
                        dutyCalculatorModel.UsedVehicleDutyResultForm.ProcessingFeeafterdedution = num.ToString("N");
                    }
                    decimal num22 = (new decimal(15) / num6) * (num18 + num21);
                    dutyCalculatorModel.UsedVehicleDutyResultForm.vatafterdedution = num22.ToString("N");
                    hDV = decimal.Parse(hSCode.ImportNHIL);
                    decimal num23 = (decimal.Parse(hDV.ToString()) / num6) * (num18 + num21);
                    dutyCalculatorModel.UsedVehicleDutyResultForm.nhilafterdedution = num23.ToString("N");
                    decimal num24 = (num21 + num22) / 48;
                    dutyCalculatorModel.UsedVehicleDutyResultForm.interestChargeafterdedution = num24.ToString("N");
                    string str4 = "0.75";
                    decimal num25 = (num18 * decimal.Parse(str4)) / num6;
                    dutyCalculatorModel.UsedVehicleDutyResultForm.ecowasafterdedution = num25.ToString("N");
                    decimal num26 = decimal.Parse(num10.ToString());
                    rate = exchangeRate.Rate;
                    decimal num27 = num26 * decimal.Parse(rate.ToString());
                    decimal num28 = (decimal.Parse("0.4") / num6) * num27;
                    dutyCalculatorModel.UsedVehicleDutyResultForm.networkChargeafterdedution = num28.ToString("N");
                    decimal num29 = (num18 * decimal.Parse(str4)) / num6;
                    dutyCalculatorModel.UsedVehicleDutyResultForm.edifafterdedution = num29.ToString("N");
                    decimal num30 = num28 * (new decimal(15) / num6);
                    dutyCalculatorModel.UsedVehicleDutyResultForm.NetchargeVatafterdedution = num30.ToString("N");
                    decimal num31 = num18 * decimal.Parse(str3);
                    dutyCalculatorModel.UsedVehicleDutyResultForm.examfeeafterdedution = num31.ToString("N");
                    string str5 = "2.5";
                    decimal num32 = num28 * (decimal.Parse(str5) / num6);
                    dutyCalculatorModel.UsedVehicleDutyResultForm.NetchargeNhilafterdedution = num32.ToString("N");
                    string str6 = "2.0";
                    decimal num33 = num18 * (decimal.Parse(str6) / num6);
                    dutyCalculatorModel.UsedVehicleDutyResultForm.specialLevyafterdedutiony = num33.ToString("N");
                    hDV = vFreightOverage1.OverAgeRate;
                    decimal num34 = num18 * (decimal.Parse(hDV.ToString()) / num6);
                    dutyCalculatorModel.UsedVehicleDutyResultForm.Overageafterdedution = num34.ToString("N");
                    decimal num35 = new decimal(5);

                    decimal num36 = (num21 + num22  + num23 + num25 + num29 + num34 + num31 + num24 + num33 + num + num28 + num30 + num32) + num35;
                    dutyCalculatorModel.UsedVehicleDutyResultForm.localCurrencyafterdedution = num36.ToString("N");
                    hDV = valuationTPD.HDV;
                    decimal num37 = decimal.Parse(hDV.ToString()) - num8;
                    dutyCalculatorModel.UsedVehicleDutyResultForm.FOB = num37;
                    double num38 = 0.875;
                    decimal num39 = decimal.Parse(num37.ToString());


                    hDV = sidFreightRate.FreightRate;
                    decimal num40 = (num39 + decimal.Parse(hDV.ToString())) * (decimal.Parse(num38.ToString()) / num6);
                    dutyCalculatorModel.UsedVehicleDutyResultForm.insurance = decimal.Parse(num40.ToString("N"));
                    decimal num41 = decimal.Parse(num40.ToString());
                    hDV = sidFreightRate.FreightRate;
                    decimal num42 = decimal.Parse(hDV.ToString());
                    decimal num43 = (num37 + num41) + num42;
                    dutyCalculatorModel.UsedVehicleDutyResultForm.cif = string.Concat(num43.ToString("N"), str, exchangeRate.CurrencyCode);
                    string str7 = "CEDI";
                    rate = exchangeRate.Rate;
                    decimal num44 = decimal.Parse(rate.ToString());
                    decimal num45 = num43 * num44;
                    decimal num46 = Math.Round(num45, 2);
                    dutyCalculatorModel.UsedVehicleDutyResultForm.ghcedis = string.Concat(str7, str, num46.ToString("N"));
                    hDV = decimal.Parse(hSCode.ImportDuty);
                    decimal num47 = decimal.Parse(hDV.ToString());
                    decimal num48 = (decimal.Parse(num47.ToString()) / num6) * num45;
                    dutyCalculatorModel.UsedVehicleDutyResultForm.Importduty = num48.ToString("N");
                    string str8 = "0.01";
                    if (num48 <= decimal.Zero)
                    {
                        num1 = num45 * decimal.Parse(str8);
                        dutyCalculatorModel.UsedVehicleDutyResultForm.ProcessingFee = num1.ToString("N");
                    }
                    else
                    {
                        num1 = new decimal();
                        dutyCalculatorModel.UsedVehicleDutyResultForm.ProcessingFee = num1.ToString("N");
                    }
                    decimal num49 = (new decimal(15) / num6) * (num45 + num48);
                    dutyCalculatorModel.UsedVehicleDutyResultForm.vat = num49.ToString("N");
                    hDV = decimal.Parse(hSCode.ImportNHIL);
                    decimal num50 = (decimal.Parse(hDV.ToString()) / num6) * (num45 + num48);
                    dutyCalculatorModel.UsedVehicleDutyResultForm.nhil = num50.ToString("N");
                    decimal num51 = (num48 + num49) / 48;
                    dutyCalculatorModel.UsedVehicleDutyResultForm.interestCharge = num51.ToString("N");
                    string str9 = "0.75";
                    decimal num52 = (num45 * decimal.Parse(str9)) / num6;
                    dutyCalculatorModel.UsedVehicleDutyResultForm.ecowas = num52.ToString("N");
                    decimal num53 = decimal.Parse(num37.ToString());
                    rate = exchangeRate.Rate;
                    decimal num54 = num53 * decimal.Parse(rate.ToString());
                    decimal num55 = (decimal.Parse("0.4") / num6) * num54;
                    dutyCalculatorModel.UsedVehicleDutyResultForm.networkCharge = num55.ToString("N");
                    decimal num56 = (num45 * decimal.Parse(str9)) / num6;
                    dutyCalculatorModel.UsedVehicleDutyResultForm.edif = num56.ToString("N");
                    decimal num57 = num55 * (new decimal(15) / num6);
                    dutyCalculatorModel.UsedVehicleDutyResultForm.NetchargeVat = num57.ToString("N");
                    decimal num58 = num45 * decimal.Parse(str8);
                    dutyCalculatorModel.UsedVehicleDutyResultForm.examfee = num58.ToString("N");
                    string str10 = "2.5";
                    decimal num59 = num55 * (decimal.Parse(str10) / num6);
                    dutyCalculatorModel.UsedVehicleDutyResultForm.NetchargeNhil = num59.ToString("N");
                    string str11 = "2.0";
                    decimal num60 = num45 * (decimal.Parse(str11) / num6);
                    dutyCalculatorModel.UsedVehicleDutyResultForm.specialLevy = num60.ToString("N");
                    hDV = vFreightOverage1.OverAgeRate;
                    decimal num61 = num45 * (decimal.Parse(hDV.ToString()) / num6);
                    dutyCalculatorModel.UsedVehicleDutyResultForm.Overage = num61.ToString("N");
                    decimal num62 = new decimal(5);
                    decimal num63 = (num48 + num49 + num50 + num52 + num56 + num61 + num58 + num51 + num60 + num1 + num55 + num57 + num59) + num62;
                    dutyCalculatorModel.UsedVehicleDutyResultForm.localCurrency = num63.ToString("N");
                    dutyCalculatorModel.ValuationTPD = valuationTPD;
                    action = base.View(dutyCalculatorModel);
                }
                else
                {
                    base.TempData["messageType"] = "danger";
                    base.TempData["message"] = "Cannot find the hscode of ValuationTPD in HSCode table (Collaboration)";
                    action = base.RedirectToAction("Index");
                }
            }
            catch (Exception exception1)
            {
                Exception exception = exception1;
                base.TempData["messageType"] = "danger";
                base.TempData["message"] = "An Error occur. Please try again later or contact the system administrator";
                ErrorSignal.FromCurrentContext().Raise(exception);
                action = base.RedirectToAction("GeneralGood");
            }
            return action;
        }

        public ActionResult CalculateDuty(int Id)
        {
            try
            {
                DutyCalculatorModel model = new DutyCalculatorModel();
                var GetTDP = paardb.ValuationTPD.Where(x => x.ID == Id).FirstOrDefault();
                decimal CC = decimal.Parse(GetTDP.EngineCapacity.ToString());

                #region other methods used               



                var GetCurrency = (from c in paardb.ExchangeRate where c.CurrencyCode == GetTDP.Currency select c).OrderByDescending(c => c.Week).FirstOrDefault();
                var GetHSCode = (from c in swdb.br_HSCodeTariff where c.HSCode==GetTDP.AssessedHSCode select c).FirstOrDefault();
                if(GetHSCode==null)
                {
                    TempData["messageType"] = "danger";
                    TempData["message"] = "Cannot find the hscode of ValuationTPD in HSCode table (Collaboration)";
                  //  Elmah.ErrorSignal.FromCurrentContext().Raise("Cannot find the hscode of ValuationTPD in HSCode table (Collaboration)");
                    return RedirectToAction("Index");
                }
                v_FreightOverage fvalue = new v_FreightOverage();              

                string space = " ";
                var curMonth = DateTime.Now.Month;
                string half = "First";
                if (curMonth > 6)
                    half = "Second";
                model.UsedVehicleDutyResultForm = new UsedVehicleDutyResultForm();
                model.UsedVehicleDutyResultForm.rate = GetCurrency.Rate.ToString();
                model.UsedVehicleDutyResultForm.shppers = "5.00";

                int age = DateTime.Now.Year - int.Parse(GetTDP.ManufactureYear);

                

                //var freightreates = dbclass.v_FreightOverage.Where(x => x.typename == GetTDP.VehicleType).ToList();
                //if (freightreates.Count > 1)
                //{
                //    freightreates = dbclass.v_FreightOverage.Where(x => x.typename == GetTDP.VehicleType && x.minimumcc <= CC && x.maximumcc >= CC).ToList();// freightreates.Where(x => x.typename == GetTDP.VehicleType && x.minimumcc <= CC && x.maximumcc >= CC).ToList();
                //}
                //if (freightreates.Count >= 1)
                //{
                //    fvalue = freightreates.FirstOrDefault();
                //}
                var GetVehicleType = paardb.sid_VehicleTypes.Where(x => x.TypeName == GetTDP.VehicleType).FirstOrDefault();
                if(GetVehicleType==null)
                {

                }
                  var GetOverAge = dbclass.v_FreightOverage.Where(x => x.typename == GetTDP.VehicleType && age >= x.MinimumAge && age <= x.MaximumAge).FirstOrDefault();
                  var freightreates = this.paardb.sid_FreightRates.Where(x => x.VehicleCategoryId == GetVehicleType.VehicleCategoryId).ToList();
                if (freightreates.Count > 1)
                {
                    freightreates = freightreates.Where(x => x.VehicleCategoryId == GetVehicleType.VehicleCategoryId && x.MinimumCC <= CC && x.MaximumCC >= CC).ToList();
                }

                sid_FreightRates row = new sid_FreightRates();
                if (freightreates.Count >= 1)
                {
                    row = freightreates.FirstOrDefault();
                }

                model.UsedVehicleDutyResultForm.freight = row.FreightRate.ToString();
                    //fvalue.freightrate.ToString();

                DepreciationClass newclass = new DepreciationClass();
                var resp = newclass.CalDepreciation(int.Parse(GetTDP.ManufactureYear), decimal.Parse(GetTDP.HDV.ToString()), half);
                model.UsedVehicleDutyResultForm.depreciation= resp.ToString();

                #endregion


                int num = 100;
                #region /////////////////////FOB Calculation  start here///////////
                decimal TotalFOB;
                decimal hdv = decimal.Parse(GetTDP.HDV.ToString());
                TotalFOB = hdv - resp;
                model.UsedVehicleDutyResultForm.FOB = TotalFOB;
                #endregion// //////////////////FOB Calculation ends here///////////////////

                #region /////////////////////Insurance Calculation  start here///////////
                decimal TotalInsurance;
                double ivalues = 0.875;
                TotalInsurance = (decimal.Parse(TotalFOB.ToString()) + decimal.Parse(row.FreightRate.ToString())) * (decimal.Parse(ivalues.ToString()) / num);
                model.UsedVehicleDutyResultForm.insurance = decimal.Parse(TotalInsurance.ToString("N"));
                #endregion ////////////////////Insurance Calculation  end here///////////

                #region ////////////////////CIF Calculation start here/////////////
                decimal TotalCIF;
                decimal InsuranceValue = decimal.Parse(TotalInsurance.ToString());
                decimal FrightValue = decimal.Parse(row.FreightRate.ToString());
                TotalCIF = TotalFOB + InsuranceValue + FrightValue;
                model.UsedVehicleDutyResultForm.cif= TotalCIF.ToString("N") + space + GetCurrency.CurrencyCode;
                #endregion ////////////////////CIF Calculation end here/////////////

                #region ////////////////////CIF Calculation start here in Local Currency/////////////
                string LocalCurrency = "CEDI";
                decimal TotalCIFLocal;
                decimal ExRate = decimal.Parse(GetCurrency.Rate.ToString());
                TotalCIFLocal = TotalCIF * ExRate;
                decimal Val = Math.Round(TotalCIFLocal, 2);
                model.UsedVehicleDutyResultForm.ghcedis = LocalCurrency + space + Val.ToString("N");
                #endregion ////////////////////CIF Calculation end here in Local Currency/////////////

                #region////////////////////Import Duty Calculation start here /////////////
                decimal TotalImportDuty;

                decimal ImportDutyValue = decimal.Parse(GetHSCode.ImportDuty.ToString());
                TotalImportDuty = (decimal.Parse(ImportDutyValue.ToString()) / num) * TotalCIFLocal;
                model.UsedVehicleDutyResultForm.Importduty = TotalImportDuty.ToString("N");
                #endregion ////////////////////Import Duty Calculation end here /////////////

                #region ////////////////////Processing Fee Calculation start here /////////////
                decimal TotalProcessingFee;
                string ProcessingFeeValue = "0.01";
                if (TotalImportDuty > 0)
                {

                    TotalProcessingFee = 0;
                    model.UsedVehicleDutyResultForm.ProcessingFee= TotalProcessingFee.ToString("N");
                }
                else
                {


                    TotalProcessingFee = (TotalCIFLocal * decimal.Parse(ProcessingFeeValue));
                    model.UsedVehicleDutyResultForm.ProcessingFee = TotalProcessingFee.ToString("N");
                }

                #endregion ////////////////////Processing Fee Calculation end here /////////////

                #region   ////////////////////Vat Calculation start here /////////////
                decimal TotalVat;
                decimal vatRate = 15;
                TotalVat = (vatRate / num) * (TotalCIFLocal + TotalImportDuty);
                model.UsedVehicleDutyResultForm.vat = TotalVat.ToString("N");
                #endregion ////////////////////Vat Calculation end here /////////////

                #region  ////////////////////NHIL Calculation start here /////////////
                decimal TotalNHIL;
                TotalNHIL = (decimal.Parse(GetHSCode.ImportNHIL.ToString()) / num) * (TotalCIFLocal + TotalImportDuty);
                model.UsedVehicleDutyResultForm.nhil = TotalNHIL.ToString("N");
                #endregion  ////////////////////NHIL Calculation end here /////////////

                #region  ////////////////////Interest charge Calculation start here /////////////
                decimal TotalInterestCharge;
                int SNum = 48;
                TotalInterestCharge = (TotalImportDuty + TotalVat) / SNum;
                model.UsedVehicleDutyResultForm.interestCharge = TotalInterestCharge.ToString("N");
                #endregion  ////////////////////Interest charge Calculation end here /////////////

                #region  ////////////////////ECowas rate Calculation start here /////////////
                decimal TotalECowasRate;
                string eco = "0.75";
                TotalECowasRate = (TotalCIFLocal * decimal.Parse(eco)) / num;
                model.UsedVehicleDutyResultForm.ecowas= TotalECowasRate.ToString("N");
                #endregion  ////////////////////ECowas rate  Calculation end here /////////////

                #region  ////////////////////Network charge Calculation start here /////////////
                decimal TotalNetworkCharge;
                string net = "0.4";
                decimal GetFob = decimal.Parse(TotalFOB.ToString()) * decimal.Parse(GetCurrency.Rate.ToString());
                TotalNetworkCharge = (decimal.Parse(net) / num) * GetFob;
                model.UsedVehicleDutyResultForm.networkCharge = TotalNetworkCharge.ToString("N");
                #endregion  ////////////////////Network charge  Calculation end here /////////////

                #region  ////////////////////EDIF Payble Calculation start here /////////////
                decimal TotalEDIF;
                TotalEDIF = (TotalCIFLocal * decimal.Parse(eco)) / num;
                model.UsedVehicleDutyResultForm.edif = TotalEDIF.ToString("N");
                #endregion  ////////////////////EDIF Payble  Calculation end here /////////////

                #region  ////////////////////Net Charge Vat Calculation start here /////////////
                decimal TotalNetchargeVat;
                decimal netVat = 15;
                TotalNetchargeVat = TotalNetworkCharge * (netVat / num);
                model.UsedVehicleDutyResultForm.NetchargeVat = TotalNetchargeVat.ToString("N");
                #endregion  ////////////////////Net Charge Vat  Calculation end here /////////////

                #region  Exam Fee Calculations
                decimal TotalExamFee;
                TotalExamFee = (TotalCIFLocal * decimal.Parse(ProcessingFeeValue));
                model.UsedVehicleDutyResultForm.examfee = TotalExamFee.ToString("N");    
                #endregion


                #region  NetCharge NHIL Calculations
                decimal TotalNetChargeNHIL;
                string FeeValue = "2.5";
                TotalNetChargeNHIL = TotalNetworkCharge * (decimal.Parse(FeeValue) / num);
                model.UsedVehicleDutyResultForm.NetchargeNhil = TotalNetChargeNHIL.ToString("N");
                #endregion

                #region Levy Calculations
                decimal TotalLevy;                
                string AllKind = "2.0";
                TotalLevy = TotalCIFLocal * (decimal.Parse(AllKind) / num);
                model.UsedVehicleDutyResultForm.specialLevy = TotalLevy.ToString("N");


                #endregion


                #region Overage Calaclations
                decimal TotalOverAge;
                TotalOverAge = TotalCIFLocal * (decimal.Parse(GetOverAge.OverAgeRate.ToString()) / num);
                model.UsedVehicleDutyResultForm.Overage= TotalOverAge.ToString("N");
                #endregion

                #region Transaction Currency Calculations
                decimal TotalTransactionCurrency;
                decimal shipperFee = 5;

                TotalTransactionCurrency = TotalImportDuty + TotalVat + TotalNHIL + TotalECowasRate + TotalEDIF + TotalOverAge + TotalExamFee + TotalInterestCharge + TotalLevy + TotalProcessingFee + TotalNetworkCharge + TotalNetchargeVat + TotalNetChargeNHIL + shipperFee;
                model.UsedVehicleDutyResultForm.localCurrency = TotalTransactionCurrency.ToString("N");

                #endregion
                model.ValuationTPD = GetTDP;
                return PartialView("CalculateDuty", model);

            }
            catch(Exception ex)
            {
                TempData["messageType"] = "danger";
                TempData["message"] = "An Error occur. Please try again later or contact the system administrator";
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                return null;
            }
        }

        public ActionResult UsedVehicleResult(string VehicleMake, string VehicleModel, string VehicleYear)
        {
            try
            {

                DutyCalculatorModel model = new DutyCalculatorModel();

                var GetVehicleDetails1 = (from v in paardb.ValuationTPD where v.Make == VehicleMake && v.Model == VehicleModel && v.ManufactureYear == VehicleYear select v).Distinct().ToList();
                if (GetVehicleDetails1.Any())
                {
                    var Getv1 = (from v in paardb.ValuationTPD where v.Make == VehicleMake && v.Model == VehicleModel && v.ManufactureYear == VehicleYear select v).Take(1).ToList();
                    model.VehicleList = Getv1;
                }
                else
                {
                    string[] nameParts = VehicleModel.Split(' ');
                    foreach (var i in nameParts)
                    {
                        string vm = i;
                        var GetVehicleDetails2 = (from v in paardb.ValuationTPD where v.Make == VehicleMake && v.ManufactureYear == VehicleYear && (v.Model.Contains(vm)) select v).Distinct().ToList();
                        if (GetVehicleDetails2.Any())
                        {
                            var Getv2 = (from v in paardb.ValuationTPD where v.Make == VehicleMake && v.ManufactureYear == VehicleYear && (v.Model.Contains(vm)) select v).Distinct().Take(10).ToList();
                            model.VehicleList = Getv2;
                        }
                    }



                }

                return View(model);
            }
            catch (Exception ex)
            {
                TempData["messageType"] = "danger";
                TempData["message"] = "An Error occur. Please try again later or contact the system administrator";
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                return RedirectToAction("UsedVehicle");
            }
        }


        #region Private Code
        [Authorize]
        public JsonResult GetFreightRateNew(string cur, int vCategoryId, int? cc)
        {

            var freightreates = this.paardb.sid_FreightRates.Where(x => x.VehicleCategoryId == vCategoryId).ToList();
            if (freightreates.Count > 1)
            {
                freightreates = freightreates.Where(x => x.VehicleCategoryId == vCategoryId && x.MinimumCC <= cc && x.MaximumCC >= cc).ToList();
            }
           
            sid_FreightRates row = new sid_FreightRates();
            if (freightreates.Count >= 1)
            {
                row = freightreates.FirstOrDefault();
            }
            else
            {
                return Json(new { FreightRate = 0, ExchangeRateT = 0, ExchangeRateF = 0 }, JsonRequestBehavior.AllowGet);
            }

            var exchangeRateT = 0.00m;
            var exRRowT = paardb.ExchangeRate.Where(x => x.CurrencyCode == cur).OrderByDescending(x => x.Week).Take(1).FirstOrDefault();
            if (exRRowT != null)
            {
                exchangeRateT = exRRowT.Rate;
            }

            var exchangeRateF = 0.00m;
            var exRRowF = paardb.ExchangeRate.Where(x => x.CurrencyCode == "USD").OrderByDescending(x => x.Week).Take(1).FirstOrDefault();
            if (exRRowF != null)
            {
                exchangeRateF = exRRowF.Rate;
            }


            //var FreightRateLC = exchangeRate
            if (row != null)
                return Json(new { FreightRate = row.FreightRate, ExchangeRateT = exchangeRateT, ExchangeRateF = exchangeRateF }, JsonRequestBehavior.AllowGet);
            else
                return Json("", JsonRequestBehavior.AllowGet);
        }



        private int AddYear(int prefix)
        {
            int newV = 1;
            int totalY = int.Parse(prefix.ToString()) + int.Parse(newV.ToString());
            return totalY;
        }
        private int RemoveYear(int prefix)
        {
            int newV = 1;
            int totalY = prefix - int.Parse(newV.ToString());
            return totalY;
        }

        private string GetMake(int Id)
        {

            var make = swdb.VehicleReference.Where(x => x.Id == Id).FirstOrDefault();
            return make.Name;
        }

        private string GetModel(int Id, int MakeId)
        {

            var modelName = swdb.VehicleModelReference.Where(x => x.Id == Id && x.MakeId == MakeId).FirstOrDefault();
            return modelName.ModelName;
        }
        public static List<ValuationTPD> GetValidVehicles(PAARv2Entities db, string MakeName, string ModelName, string Year)
        {
            List<ValuationTPD> TPD = db.ValuationTPD.Where(x => x.Make == MakeName && x.Model == ModelName && x.ManufactureYear == Year).ToList();
            return TPD;
        }

 
        public static List<ValuationTPD> GetChassis(PAARv2Entities db, string chassis)
        {

            List<ValuationTPD> MainTPD = db.ValuationTPD.Where(x => x.ChassisNo==chassis).Take(1).ToList();            
           return MainTPD;

        }


        #endregion

        public ActionResult GetResultNone(int VehicleMake, int VehicleModel, string VehicleYear)
        {
            try
            {
                DutyCalculatorModel model = new DutyCalculatorModel();
                var GetMakeName = GetMake(VehicleMake);// swdb.VehicleReference.Where(x => x.Id == VehicleMake).FirstOrDefault();
                var GetModelName = GetModel(VehicleModel, VehicleMake); //; swdb.VehicleModelReference.Where(x => x.Id == VehicleModel).FirstOrDefault();
                string vy = VehicleYear;
                var GetVehicleDetails1 = (from v in paardb.ValuationTPD where v.Make == GetMakeName && v.Model == GetModelName && v.ManufactureYear == VehicleYear select v).Distinct().ToList();
                if (GetVehicleDetails1.Any())
                {
                    var Getv1 = (from v in paardb.ValuationTPD where v.Make == GetMakeName && v.Model == GetModelName && v.ManufactureYear == VehicleYear select v).Take(1).ToList();
                    model.VehicleList = Getv1;
                }
                else
                {
                    string[] nameParts = GetModelName.Split(' ');
                    foreach (var i in nameParts)
                    {
                        string vm = i;
                        var GetVehicleDetails2 = (from v in paardb.ValuationTPD where v.Make == GetMakeName && v.ManufactureYear == VehicleYear && (v.Model.Contains(vm)) select v).Distinct().ToList();
                        if (GetVehicleDetails2.Any())
                        {
                            var Getv2 = (from v in paardb.ValuationTPD where v.Make == GetMakeName && v.ManufactureYear == VehicleYear && (v.Model.Contains(vm)) select v).Distinct().Take(10).ToList();
                            model.VehicleList = Getv2;
                        }
                        else
                        {
                            //check for related year if model and make is found but year not found                           
                            string vYear = VehicleYear;
                            int newV = 1;
                            int totalY = int.Parse(vYear.ToString()) - int.Parse(newV.ToString());
                            string newYear = totalY.ToString();
                            var GetRelatedYear = (from v in paardb.ValuationTPD where v.Make == GetMakeName && v.ManufactureYear.StartsWith(newYear) && (v.Model.Contains(vm)) select v).Distinct().ToList();
                            if (GetRelatedYear.Any())
                            {
                                TempData["messageType"] = "success";
                                TempData["message"] = "We cannot find the year " + GetModelName + " of your vehicle. Here are related Years for your search";
                                var DisplayRelatedYear = (from v in paardb.ValuationTPD where v.Make == GetMakeName && v.ManufactureYear.StartsWith(newYear) && (v.Model.Contains(vm)) select v).Distinct().Take(5).ToList();
                                model.VehicleList = DisplayRelatedYear;
                            }

                        }
                    }

                    //check for Year

                    var GetVYear = (from v in paardb.ValuationTPD where v.Make == GetMakeName && v.ManufactureYear == VehicleYear select v).Distinct().ToList();
                    if (GetVYear.Any())
                    {
                        TempData["messageType"] = "success";
                        TempData["message"] = "We cannot find " + GetModelName + " " + VehicleYear + ". Here are related model for your search";
                        var Getv3 = (from v in paardb.ValuationTPD where v.Make == GetMakeName && v.ManufactureYear == VehicleYear select v).Distinct().Take(5).ToList();
                        model.VehicleList = Getv3;
                    }
                    else
                    {
                        //check for related Year
                        // removing 1 year from the year entered
                        int gyear = int.Parse(VehicleYear);
                        int gyear1 = RemoveYear(gyear);
                        string vYear = gyear1.ToString();

                        //adding one year to the year entered
                        int YearEntered = int.Parse(VehicleYear);
                        int YearAdded = AddYear(YearEntered);
                        string YearGotten = YearAdded.ToString();

                        //int newV = 1;
                        //int totalY = int.Parse(vYear.ToString()) - int.Parse(newV.ToString());
                        //string newYear = totalY.ToString();

                        var GetRelatedYear = (from v in paardb.ValuationTPD where v.Make == GetMakeName && v.ManufactureYear.StartsWith(vYear) || v.ManufactureYear.StartsWith(YearGotten) select v).Distinct().ToList();
                        if (GetRelatedYear.Any())
                        {
                            TempData["messageType"] = "success";
                            TempData["message"] = "We cannot find your vehicle model " + GetModelName + " and the Year " + VehicleYear + ". below are related vehicle(s) for your search";
                            var Get4 = (from v in paardb.ValuationTPD where v.Make == GetMakeName && v.ManufactureYear.StartsWith(vYear) || v.ManufactureYear.StartsWith(YearGotten) select v).Distinct().Take(5).ToList();
                            model.VehicleList = Get4;
                        }
                    }
                }

                return PartialView("SearchResult", model);
            }
            catch (Exception ex)
            {
                TempData["messageType"] = "danger";
                TempData["message"] = "An Error occur. Please try again later or contact the system administrator";
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                return null;
            }

        }

    }
}
