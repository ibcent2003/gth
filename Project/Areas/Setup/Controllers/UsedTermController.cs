using Project.Areas.Setup.Models;
using Project.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Project.Areas.Setup.Controllers
{
    public class UsedTermController : Controller
    {
        //
        // GET: /Setup/UsedTerm/
        private PROEntities db = new PROEntities();
        public ActionResult Index()
        {
            try
            {
                var rowsToShow = db.CommonlyUsedTerms.ToList();
                var viewModel = new UsedTermViewModel
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
                UsedTermViewModel model = new UsedTermViewModel();
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
        public ActionResult Create(UsedTermViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var validate = (from m in db.CommonlyUsedTerms where m.Terms == model.usedtermform.Name select m).ToList();
                    if (validate.Any())
                    {
                        TempData["messageType"] = "danger";
                        TempData["message"] = "The terms" + model.usedtermform.Name + " already exist. Please try different Name";
                        return View(model);
                    }
                    CommonlyUsedTerms addnew = new CommonlyUsedTerms
                    {
                        Terms = model.usedtermform.Name,
                        Conditions = model.usedtermform.Description,
                        ModifiedBy = User.Identity.Name,
                        ModifiedDate = DateTime.Now
                    };
                    db.CommonlyUsedTerms.AddObject(addnew);
                    db.SaveChanges();
                    TempData["message"] = "<b>" + model.usedtermform.Name + "</b> was Successfully added";
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
                UsedTermViewModel model = new UsedTermViewModel();
                var GetTerms = db.CommonlyUsedTerms.Where(x => x.Id == Id).FirstOrDefault();
                model.usedtermform = new  CommonlyUsedTermsForm();
                model.usedtermform.Name = GetTerms.Terms;
                model.usedtermform.Description = GetTerms.Conditions;
                model.usedtermform.Id = Id;
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
        public ActionResult Edit(UsedTermViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var GetTerm = db.CommonlyUsedTerms.Where(x => x.Id == model.usedtermform.Id).FirstOrDefault();
                    GetTerm.Terms = model.usedtermform.Name;
                    GetTerm.Conditions = model.usedtermform.Description;
                    GetTerm.ModifiedBy = User.Identity.Name;
                    GetTerm.ModifiedDate = DateTime.Now;
                    db.SaveChanges();
                    TempData["message"] = "<b>" + model.usedtermform.Name + "</b> was Successfully updated";
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

        public ActionResult GNSWTerms()
        {
            try
            {
                UsedTermViewModel model = new UsedTermViewModel();
                var GetGNSWTerms = db.TermsOfUsed.Where(x => x.Id == 1).FirstOrDefault();
                model.gnswterm = new  GNSWTerm();
                model.gnswterm.ContenetType = GetGNSWTerms.ContentType;
                model.gnswterm.ContenetInformation = GetGNSWTerms.ContentInformation;
                model.gnswterm.Id = 1;
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
        public ActionResult GNSWTerms(UsedTermViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var GetGNSWTerms = db.TermsOfUsed.Where(x => x.Id == 1).FirstOrDefault();
                    GetGNSWTerms.ContentType = model.gnswterm.ContenetType;
                    GetGNSWTerms.ContentInformation = model.gnswterm.ContenetInformation;                   
                    db.SaveChanges();
                    TempData["message"] = "<b>" + model.gnswterm.ContenetType + "</b> was Successfully updated";
                    return RedirectToAction("GNSWTerms");
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

        public ActionResult GNSWPrivacy()
        {
            try
            {
                UsedTermViewModel model = new UsedTermViewModel();
                var GetGNSWPrivacy = db.TermsOfUsed.Where(x => x.Id == 2).FirstOrDefault();
                model.gnswprivacy = new  GNSWPrivacy();
                model.gnswprivacy.ContenetType = GetGNSWPrivacy.ContentType;
                model.gnswprivacy.ContenetInformation = GetGNSWPrivacy.ContentInformation;
                model.gnswprivacy.Id = 1;
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
        public ActionResult GNSWPrivacy(UsedTermViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var GetGNSWPrivacy = db.TermsOfUsed.Where(x => x.Id == 2).FirstOrDefault();
                    GetGNSWPrivacy.ContentType = model.gnswprivacy.ContenetType;
                    GetGNSWPrivacy.ContentInformation = model.gnswprivacy.ContenetInformation;
                    db.SaveChanges();
                    TempData["message"] = "<b>" + model.gnswprivacy.ContenetType + "</b> was Successfully updated";
                    return RedirectToAction("GNSWPrivacy");
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
