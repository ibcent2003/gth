using Project.Areas.Admin.Models;
using Project.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Project.Areas.Admin.Controllers
{
    public class DashboardController : Controller
    {
        //
        // GET: /Admin/Dashboard/
        private PROEntities db = new PROEntities();

        //public ActionResult Index()
        //{
        //    DashboardViewModel model = new DashboardViewModel();
        //    var GetTotalDuty = db.ContactUs.Where(x => x.MessageType == "Duty").Count();
        //    var GetUserFeedback = db.ContactUs.Where(x=>x.MessageType=="Feedback").Count();
        //    var GetDocument = db.DocumentInfo.Count();
        //    var GetNews = db.News.Where(x => x.IsPublished == true && x.IsDeleted == false).Count();

        //    model.TotalNews = GetNews;
        //    model.TotalDutyFeedback = GetTotalDuty;
        //    model.TotalUserFeedback = GetUserFeedback;
        //    model.TotalDocument = GetDocument;




        //    return View(model);
        //}

        public ActionResult Index()
        {
            if (base.User.IsInRole("Agency Admin"))
            {
                return base.RedirectToAction("Index", "Dashboard", new { area = "Organisation" });
            }
            DashboardViewModel dashboardViewModel = new DashboardViewModel();
            int num = (
                from x in this.db.ContactUs
                where x.MessageType == "Duty"
                select x).Count<ContactUs>();
            int num1 = (
                from x in this.db.ContactUs
                where x.MessageType == "Feedback"
                select x).Count<ContactUs>();
            int num2 = this.db.DocumentInfo.Count<DocumentInfo>();
            int num3 = (
                from x in this.db.News
                where x.IsPublished && !x.IsDeleted
                select x).Count<News>();
            DutyCounter dutyCounter = (
                from x in this.db.DutyCounter
                where x.CounterType == "Used Vehicle"
                select x).FirstOrDefault<DutyCounter>();
            DutyCounter dutyCounter1 = (
                from x in this.db.DutyCounter
                where x.CounterType == "General Goods"
                select x).FirstOrDefault<DutyCounter>();
            DutyCounter dutyCounter2 = (
                from x in this.db.DutyCounter
                where x.CounterType == "Consigment Tracker"
                select x).FirstOrDefault<DutyCounter>();
            DutyCounter dutyCounter3 = (
                from x in this.db.DutyCounter
                where x.CounterType == "Visitors"
                select x).FirstOrDefault<DutyCounter>();
            dashboardViewModel.TotalOrg = this.db.Organization.Count<Organization>();
            dashboardViewModel.NoOfUsedDuty = dutyCounter.TotalUsed;
            dashboardViewModel.NoOfGeneralGoods = dutyCounter1.TotalUsed;
            dashboardViewModel.TotalUsersVisit = dutyCounter3.TotalUsed;
            dashboardViewModel.TotalNews = num3;
            dashboardViewModel.TotalDutyFeedback = num;
            dashboardViewModel.TotalUserFeedback = num1;
            dashboardViewModel.TotalDocument = num2;
            dashboardViewModel.TotalConsigmentUsed = dutyCounter2.TotalUsed;
            return base.View(dashboardViewModel);
        }

    }
}
