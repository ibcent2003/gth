using Project.Areas.Setup.Models;
using Project.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Project.Areas.Setup.Controllers
{
    public class EventManagementController : Controller
    {
        //
        // GET: /Setup/EventManagement/
        private PROEntities db = new PROEntities();
      
        public ActionResult Index()
        {
            try
            {
                var rowsToShow = db.Event.ToList();
                var viewModel = new EventViewModel
                {
                    Rows = rowsToShow.OrderByDescending(x => x.Name).ToList(),
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


        public ActionResult Create()
        {
            try
            {               
                EventViewModel model = new EventViewModel();
                return View(model);
            }
            catch (Exception ex)
            {
                TempData["messageType"] = "danger";
                TempData["message"] = "There is an error in the application. Please try again or contact the system administrator";
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                return RedirectToAction("Index", "Dashboard", new { area = "Admin" });
            }
        }


        [HttpPost, ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public ActionResult Create(EventViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var validate = (from m in db.Event where m.Name == model.eventform.Name select m).ToList();
                    if (validate.Any())
                    {
                        TempData["messageType"] = "danger";
                        TempData["message"] = "The Name" + model.eventform.Name + " already exist. Please try different Name";
                        return View(model);
                    }
                    Event addnew = new Event
                    {
                        Name = model.eventform.Name,
                        Venue = model.eventform.Venue,
                        EventDate = model.eventform.EventDate,
                        EventTime =model.eventform.EventTime,
                        HasEnded = model.eventform.HasEnded,
                        ModifiedBy = User.Identity.Name,
                        ModifiedDate = DateTime.Now
                        
                    };
                    db.Event.AddObject(addnew);
                    db.SaveChanges();
                    TempData["message"] = "<b>" + model.eventform.Name + "</b> was Successfully added";
                    return RedirectToAction("Index");

                }
                return View(model);
            }
            catch (Exception ex)
            {
                TempData["messageType"] = "danger";
                TempData["message"] = "There is an error in the application. Please try again or contact the system administrator";
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                return RedirectToAction("Index", "Dashboard", new { area = "Admin" });
            }
        }


        public ActionResult Edit(int Id)
        {
            try
            {
                EventViewModel model = new EventViewModel();
                var GetEvent = db.Event.Where(x => x.Id == Id).FirstOrDefault();
                model.eventform = new  EventForm();
                model.eventform.Name = GetEvent.Name;
                model.eventform.Venue = GetEvent.Venue;
                model.eventform.EventDate = GetEvent.EventDate;
                model.eventform.EventTime = GetEvent.EventTime;
                model.eventform.HasEnded = GetEvent.HasEnded;
                model.eventform.Id = Id;
                return View(model);

            }
            catch (Exception ex)
            {
                TempData["messageType"] = "danger";
                TempData["message"] = "There is an error in the application. Please try again or contact the system administrator";
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                return RedirectToAction("Index", "Dashboard", new { area = "Admin" });
            }
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(EventViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var GetEvent = db.Event.Where(x => x.Id == model.eventform.Id).FirstOrDefault();
                    GetEvent.Name = model.eventform.Name;
                    GetEvent.Venue = model.eventform.Venue;
                    GetEvent.EventDate = model.eventform.EventDate;
                    GetEvent.EventTime = model.eventform.EventTime;
                    GetEvent.HasEnded = model.eventform.HasEnded;
                    GetEvent.ModifiedDate = DateTime.Now;
                    GetEvent.ModifiedBy = User.Identity.Name;
                    db.SaveChanges();
                    TempData["message"] = "<b>" + model.eventform.Name + "</b> was Successfully updated";
                    return RedirectToAction("Index");
                }
                return View(model);
            }
            catch (Exception ex)
            {
                TempData["messageType"] = "danger";
                TempData["message"] = "There is an error in the application. Please try again or contact the system administrator";
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                return RedirectToAction("Index", "Dashboard", new { area = "Admin" });
            }
        }

    }
}
