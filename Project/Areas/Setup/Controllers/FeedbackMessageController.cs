using Project.Areas.Setup.Models;
using Project.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Project.Areas.Setup.Controllers
{
    public class FeedbackMessageController : Controller
    {
        //
        // GET: /Setup/FeedbackMessage/
        private PROEntities db = new PROEntities();

        public ActionResult Feedback()
        {
            try
            {
                var rowsToShow = db.ContactUs.Where(x=>x.MessageType=="Feedback").ToList();
                var viewModel = new FeedbackMessageViewModel
                {
                    Rows = rowsToShow.OrderByDescending(x => x.SentDate).ToList(),
                };
                return View(viewModel);
            }
            catch (Exception ex)
            {
                TempData["messageType"] = "alert-danger";
                TempData["message"] = "There is an error in the application. Please try again or contact the system administrator";
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                return RedirectToAction("Index", "Dashboard", new { area = "Admin" });
            }
        }

        public ActionResult DutyFeedback()
        {
            try
            {
                var rowsToShow = db.ContactUs.Where(x => x.MessageType == "Duty").ToList();
                var viewModel = new FeedbackMessageViewModel
                {
                    Rows = rowsToShow.OrderByDescending(x => x.SentDate).ToList(),
                };
                return View(viewModel);
            }
            catch (Exception ex)
            {
                TempData["messageType"] = "alert-danger";
                TempData["message"] = "There is an error in the application. Please try again or contact the system administrator";
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                return RedirectToAction("Index", "Dashboard", new { area = "Admin" });
            }
        }

    }
}
