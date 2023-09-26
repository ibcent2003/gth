using Project.Areas.Setup.Models;
using Project.DAL;
using Project.Models;
using Project.Properties;
using Project.UI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;


namespace Project.Areas.Setup.Controllers
{
    public class UsefulLinkManagementController : Controller
    {
        //
        // GET: /Setup/UsefulLinkManagement/
        private PROEntities db = new PROEntities();
        public ActionResult Index()
        {
            try
            {
                var rowsToShow = db.UsefulLink.ToList();
                var viewModel = new UsefullinkViewModel
                {
                    Rows = rowsToShow.OrderByDescending(x => x.ModifiedDate).ToList(),
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
                UsefullinkViewModel model = new UsefullinkViewModel();
                return View(model);
            }
            catch (Exception ex)
            {
                TempData["messageType"] = "alert-danger";
                TempData["message"] = "There is an error in the application. Please try again or contact the system administrator";
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                return RedirectToAction("Index", "Dashboard", new { area = "Admin" });
            }
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(UsefullinkViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {

                    var validate = (from m in db.UsefulLink where m.Name == model.UsefulLinkform.Name select m).ToList();
                    if (validate.Any())
                    {
                        TempData["messageType"] = "danger";
                        TempData["message"] = "The Name" + model.UsefulLinkform.Name + " already exist. Please try different Name";
                        return View(model);
                    }
                 
                    UsefulLink add = new UsefulLink
                    {
                        Name = model.UsefulLinkform.Name,
                        Link = model.UsefulLinkform.Link,                       
                        ModifiedBy = User.Identity.Name,
                        ModifiedDate = DateTime.Now,
                    };
                    db.UsefulLink.AddObject(add);
                    db.SaveChanges();
                    TempData["message"] = "<b>" + model.UsefulLinkform.Name + "</b> was Successfully created";
                    return RedirectToAction("Index");

                }
                return View(model);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                TempData["messageType"] = "danger";
                TempData["message"] = Settings.Default.GenericExceptionMessage;
                return RedirectToAction("Index", "Dashboard", new { area = "Admin" });
            }
        }


        public ActionResult Edit(int Id)
        {
            try
            {
                UsefullinkViewModel model = new UsefullinkViewModel();
                var Getuseful = db.UsefulLink.Where(x => x.Id == Id).FirstOrDefault();
                model.UsefulLinkform = new  UsefulLinkForm();
                model.UsefulLinkform.Name = Getuseful.Name;
                model.UsefulLinkform.Link = Getuseful.Link;
                model.UsefulLinkform.Id = Id;
                return View(model);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                TempData["messageType"] = "danger";
                TempData["message"] = Settings.Default.GenericExceptionMessage;
                return RedirectToAction("Index", "Home", new { area = "Admin" });
            }
        }

        [HttpPost]
        public ActionResult Edit(UsefullinkViewModel model)
        {
            try
            {
                var Getuseful = db.UsefulLink.Where(x => x.Id == model.UsefulLinkform.Id).FirstOrDefault();

                    Getuseful.Name = model.UsefulLinkform.Name;
                    Getuseful.Link = model.UsefulLinkform.Link;
                    Getuseful.ModifiedBy = User.Identity.Name;
                    Getuseful.ModifiedDate = DateTime.Now;
                    db.SaveChanges();
                    TempData["message"] = "<b>" + model.UsefulLinkform.Name + "</b> was Successfully updated";
                    return RedirectToAction("Index");
                
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                TempData["messageType"] = "danger";
                TempData["message"] = Settings.Default.GenericExceptionMessage;
                return RedirectToAction("Index", "Home", new { area = "Admin" });
            }
        }
    }
}
