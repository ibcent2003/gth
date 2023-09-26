using Project.Areas.Setup.Models;
using Project.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Project.Areas.Setup.Controllers
{
    public class FAQManagementController : Controller
    {
        private PROEntities db = new PROEntities();
        // GET: /Setup/FAQManagement/

        public ActionResult Index()
        {
            try
            {
                var rowsToShow = db.FAQ.ToList();
                var viewModel = new  FAQViewModel
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
                //if (ModelState.IsValid)
                //{
                //}
                FAQViewModel model = new FAQViewModel();
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
        public ActionResult Create(FAQViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var validate = (from m in db.FAQ where m.Question == model.faqform.Question select m).ToList();
                    if (validate.Any())
                    {                        
                        TempData["messageType"] = "danger";
                        TempData["message"] = "The Question" + model.faqform.Question + " already exist. Please try different Question";
                        return View(model);
                    }
                    FAQ addnew = new FAQ
                    {
                        Question = model.faqform.Question,
                        Answer = model.faqform.Answer,
                        ModifiedBy = User.Identity.Name,
                        ModifiedDate = DateTime.Now
                    };
                    db.FAQ.AddObject(addnew);
                    db.SaveChanges();
                    TempData["message"] = "<b>" + model.faqform.Question + "</b> was Successfully added";
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
             FAQViewModel model = new FAQViewModel();
             var GetFAQ = db.FAQ.Where(x => x.Id == Id).FirstOrDefault();
             model.faqform = new FAQForm();
             model.faqform.Question = GetFAQ.Question;
             model.faqform.Answer = GetFAQ.Answer;
             model.faqform.Id = Id;
             return View(model);

            }
            catch(Exception ex)
            {
                TempData["messageType"] = "danger";
                TempData["message"] = "There is an error in the application. Please try again or contact the system administrator";
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                return RedirectToAction("Index", "Dashboard", new { area = "Admin" });
            }
        }


        [HttpPost, ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(FAQViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var GetFAQ = db.FAQ.Where(x => x.Id == model.faqform.Id).FirstOrDefault();
                    GetFAQ.Question = model.faqform.Question;
                    GetFAQ.Answer = model.faqform.Answer;
                    GetFAQ.ModifiedBy = User.Identity.Name;
                    GetFAQ.ModifiedDate = DateTime.Now;
                    db.SaveChanges();
                    TempData["message"] = "<b>" + model.faqform.Question + "</b> was Successfully updated";
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
