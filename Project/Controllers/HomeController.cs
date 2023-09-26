using Elmah;
using PAARS.DAL;
using Project.DAL;
using Project.Models;
using Project.Properties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;


namespace Project.Controllers
{
    public class HomeController : Controller
    {
        private PAARv2Entities paardb = new PAARv2Entities();

        private PROEntities db = new PROEntities();

        public ActionResult Index(IndexViewModel model)
        {
            //try
            //{

            //    model.NewsList = db.News.Where(x => x.IsPublished == true && x.IsDeleted == false).ToList();
            //     model.PicturePath = Settings.Default.PhotoPath;
            //    model.masterURL = Settings.Default.MasterUrl;
            //    var GetEvent = (from f in db.Event where f.HasEnded == false select f).OrderByDescending(x => x.Id).FirstOrDefault();
            //    if (GetEvent == null)
            //    {
            //        model.HasNewEvent = false;
            //    }
            //    else
            //    {
            //        model.EventInformation = GetEvent;
            //        model.HasNewEvent = true;
            //    }
            //    return View(model);
            //}
            //catch (Exception ex)
            //{
            //    TempData["messageType"] = "alert-danger";
            //    Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
            //    return RedirectToAction("Error404");
            //}

            ActionResult action;
            try
            {
                model.NewsList = (
                    from x in this.db.News
                    where x.IsPublished && !x.IsDeleted
                    select x).ToList<News>();
                model.PicturePath = Settings.Default.PhotoPath;
                model.masterURL = Settings.Default.MasterUrl;
                Event @event = (
                    from f in this.db.Event
                    where !f.HasEnded
                    select f into x
                    orderby x.Id descending
                    select x).FirstOrDefault<Event>();
                if (@event != null)
                {
                    model.EventInformation = @event;
                    model.HasNewEvent = true;
                }
                else
                {
                    model.HasNewEvent = false;
                }
                var totalUsed = (
                    from x in this.db.DutyCounter
                    where x.CounterType == "Visitors"
                    select x).FirstOrDefault();
                totalUsed.TotalUsed = totalUsed.TotalUsed + 1;
                int num = (
                    from x in this.db.DutyCounter
                    where x.CounterType == "Classic View"
                    select x).FirstOrDefault().TotalUsed;
                this.db.SaveChanges();
                model.Testimonials = (
                    from x in this.db.Testimonials
                    where x.IsPulished && !x.HasDeleted
                    orderby x.Id descending
                    select x).ToList<Testimonials>();
                action = base.View(model);
            }
            catch (Exception exception1)
            {
                Exception exception = exception1;
                base.TempData["messageType"] = "alert-danger";
                ErrorSignal.FromCurrentContext().Raise(exception);
                action = base.RedirectToAction("Error404");
            }
            return action;

        }

        public ActionResult Classic(IndexViewModel model)
        {

            //try
            //{

            //    model.NewsList = db.News.Where(x=>x.IsPublished==true && x.IsDeleted==false).ToList();
            //    model.PicturePath = Settings.Default.PhotoPath;
            //    var GetEvent = (from f in db.Event where f.HasEnded==false select f).OrderByDescending(x=>x.Id).FirstOrDefault();
            //    if(GetEvent==null)
            //    {
            //        model.HasNewEvent = false;
            //    }
            //    else
            //    {
            //        model.EventInformation = GetEvent;
            //        model.HasNewEvent = true;
            //    }               
            //    return View(model);
            //}
            //catch (Exception ex)
            //{              
            //    TempData["messageType"] = "alert-danger";
            //    Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
            //    return RedirectToAction("Error404");
            //}


            ActionResult action;
            try
            {
                model.NewsList = (
                    from x in this.db.News
                    where x.IsPublished && !x.IsDeleted
                    select x).ToList<News>();
                model.PicturePath = Settings.Default.PhotoPath;
                model.masterURL = Settings.Default.MasterUrl;
                Event @event = (
                    from f in this.db.Event
                    where !f.HasEnded
                    select f into x
                    orderby x.Id descending
                    select x).FirstOrDefault<Event>();
                if (@event != null)
                {
                    model.EventInformation = @event;
                    model.HasNewEvent = true;
                }
                else
                {
                    model.HasNewEvent = false;
                }
                DutyCounter totalUsed = (
                    from x in this.db.DutyCounter
                    where x.CounterType == "Visitors"
                    select x).FirstOrDefault<DutyCounter>();
                totalUsed.TotalUsed = totalUsed.TotalUsed + 1;
                int num = (
                    from x in this.db.DutyCounter
                    where x.CounterType == "Classic View"
                    select x).FirstOrDefault<DutyCounter>().TotalUsed;
                this.db.SaveChanges();
                model.Testimonials = (
                    from x in this.db.Testimonials
                    where x.IsPulished && !x.HasDeleted
                    orderby x.Id descending
                    select x).ToList<Testimonials>();
                action = base.View(model);
            }
            catch (Exception exception1)
            {
                Exception exception = exception1;
                base.TempData["messageType"] = "alert-danger";
                ErrorSignal.FromCurrentContext().Raise(exception);
                action = base.RedirectToAction("Error404");
            }
            return action;
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your app description page.";

            return View();
        }

        public ActionResult Contact()
        {
            IndexViewModel model = new IndexViewModel();
            return View(model);
        }

        [HttpPost]
        public ActionResult Contact(IndexViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    ContactUs addnew = new ContactUs
                    {
                        Fullname = model.feedbackform.full_name,
                        EmailAddress = model.feedbackform.Email,
                        PhoneNo = model.feedbackform.MobileNumber,
                        Message = model.feedbackform.MessageInput,
                        SentDate = DateTime.Now
                    };
                    db.ContactUs.AddObject(addnew);
                    db.SaveChanges();
                }                   
                return View(model);
            }
            catch(Exception ex)
            {
                TempData["messageType"] = "danger";
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                return RedirectToAction("Error404");
            }
        }

        [HttpPost]
        public ActionResult AddFeedback(IndexViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    DutyCalculatorFeedback addnew = new DutyCalculatorFeedback
                    {
                        FullName = model.Dutyfeedbacform.FullName,
                        EmailAddress = model.Dutyfeedbacform.EmailAddress,
                        Message = model.Dutyfeedbacform.Message,
                        SentDate = DateTime.Now
                    };
                    db.DutyCalculatorFeedback.AddObject(addnew);
                    db.SaveChanges();
                    TempData["messageType"] = "success";
                    TempData["message"] = "Feedback has been sent successfully";
                    return RedirectToAction("Contact");
                }
                catch (Exception ex)
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
        }

        public ActionResult ViewNews(int Id=0)
        {
            try
            {
                if (Id == 0)
                {
                    TempData["messageType"] = "alert-danger";                   
                    Elmah.ErrorSignal.FromCurrentContext().Raise(new Exception("No Service Id attach to this request"));
                    return RedirectToAction("Error404");
                }
                int add = 1;
                var GetNews = db.News.Where(x => x.Id == Id).FirstOrDefault();

                IndexViewModel model = new IndexViewModel();
                int oldCount = GetNews.NoOfView;
                int totalCount = add + oldCount;
                GetNews.NoOfView = totalCount;
                db.SaveChanges();
                model.news = GetNews;
                model.PicturePath = Settings.Default.PhotoPath;
                model.NewsList = (from n in db.News select n).Take(5).ToList();
                var GetNew = (from s in db.News select s).Distinct().ToList();
                List<DateTime> result = GetNew.Select(d => new DateTime(d.CreatedDate.Year, d.CreatedDate.Month, 1)).Distinct().ToList();
                model.newslist = result;

                
                model.newsCategory = (from n in db.News select n.NewsCategory.Name).Distinct().ToList();           
                return View(model);
            }
            catch(Exception ex)
            {
                TempData["messageType"] = "alert-danger";
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                return RedirectToAction("Error404");
            }
        }


        public ActionResult AdminNewsView(int Id=0)
        {
            try
            {
                if (Id == 0)
                {
                    TempData["messageType"] = "alert-danger";
                    Elmah.ErrorSignal.FromCurrentContext().Raise(new Exception("No Service Id attach to this request"));
                    return RedirectToAction("Error404");
                }            
                var GetNews = db.News.Where(x => x.Id == Id).FirstOrDefault();

                IndexViewModel model = new IndexViewModel();              
                model.news = GetNews;
                model.PicturePath = Settings.Default.PhotoPath;
                model.NewsList = (from n in db.News select n).Take(5).ToList();
                var GetNew = (from s in db.News select s).Distinct().ToList();
                List<DateTime> result = GetNew.Select(d => new DateTime(d.CreatedDate.Year, d.CreatedDate.Month, 1)).Distinct().ToList();
                model.newslist = result;


                model.newsCategory = (from n in db.News select n.NewsCategory.Name).Distinct().ToList();
                return View(model);
            }
            catch (Exception ex)
            {
                TempData["messageType"] = "alert-danger";
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                return RedirectToAction("Error404");
            }
        }
        public ActionResult Error404()
        {
            return View();
        }


        public ActionResult CurrencyConverter(IndexViewModel model)
        {
            try
            {
                model.Currencies = new SelectList(paardb.ExchangeRateView.OrderByDescending(x => x.Week).Take(18), "Rate", "CurrencyName");
                model.CurrencyList = paardb.ExchangeRateView.OrderByDescending(x => x.Week).Take(18).ToList();
                return View(model);
            }
            catch(Exception ex)
            {
                TempData["messageType"] = "alert-danger";
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                return RedirectToAction("Error404");
            }
        }


        public ActionResult CpcList(IndexViewModel model)
        {
            try
            {
                model.CPCList = db.CPC.OrderByDescending(x => x.Description).ToList();
                return View(model);
            }
            catch(Exception ex)
            {
                TempData["messageType"] = "alert-danger";
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                return RedirectToAction("Error404");
            }
        }

        public ActionResult ImportClassification()
        {
            return View();
        }
    }
}
