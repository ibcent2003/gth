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
    public class ProceeduresManagementController : Controller
    {
        private PROEntities db = new PROEntities();
        // GET: /Setup/ProceeduresManagement/

        public ActionResult Index()
        {
            try
            {
                var rowsToShow = db.ImportExportProcedure.ToList();
                var viewModel = new ProceedureViewModel
                {
                    Rows = rowsToShow.OrderBy(x => x.Name).ToList(),
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
                ProceedureViewModel model = new ProceedureViewModel();
                model.ProcedureTypeList = (from s in db.ProcedureType select new IntegerSelectListItem { Text = s.Name, Value = s.Id }).ToList();
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
        public ActionResult Create(ProceedureViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {

                    var validate = (from m in db.ProductType where m.Name == model.proceeduresForm.Name select m).ToList();
                    if (validate.Any())
                    {
                        model.ProcedureTypeList = (from s in db.ProcedureType select new IntegerSelectListItem { Text = s.Name, Value = s.Id }).ToList();
                        TempData["messageType"] = "alert-danger";
                        TempData["message"] = "The Name" + model.proceeduresForm.Name + " already exist. Please try different Name";
                        return View(model);
                    }                  
                    ImportExportProcedure add = new ImportExportProcedure
                    {
                        Name = model.proceeduresForm.Name,                     
                        Description = model.proceeduresForm.Description,
                        ProcedureTypeId = model.proceeduresForm.ProcedureTypeId,
                        ModifiedBy = User.Identity.Name,
                        ModifiedDate = DateTime.Now,
                    };
                    db.ImportExportProcedure.AddObject(add);
                    db.SaveChanges();
                    TempData["message"] = "<b>" + model.proceeduresForm.Name + "</b> was Successfully created";
                    return RedirectToAction("Index");

                }
                model.ProcedureTypeList = (from s in db.ProcedureType select new IntegerSelectListItem { Text = s.Name, Value = s.Id }).ToList();
                return View(model);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                TempData["messageType"] = "alert-danger";
                TempData["message"] = Settings.Default.GenericExceptionMessage;
                return RedirectToAction("Index", "Dashboard", new { area = "Admin" });
            }
        }



        public ActionResult Edit(int Id)
        {
            try
            {

                ProceedureViewModel model = new ProceedureViewModel();
                var GetProceedure = db.ImportExportProcedure.Where(x => x.Id == Id).FirstOrDefault();
                model.ProcedureTypeList = (from s in db.ProcedureType select new IntegerSelectListItem { Text = s.Name, Value = s.Id }).ToList();
                model.proceeduresForm = new Models.ProceeduresForm();
                model.proceeduresForm.Name = GetProceedure.Name;
                model.proceeduresForm.Description = GetProceedure.Description;
                model.proceeduresForm.Id = Id;
                model.proceeduresForm.ProcedureTypeId = GetProceedure.ProcedureTypeId;                
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
        public ActionResult Edit(ProceedureViewModel model)
        {
            try
            {

                if (ModelState.IsValid)
                {
                    var GetProceedure = db.ImportExportProcedure.Where(x => x.Id == model.proceeduresForm.Id).FirstOrDefault();
                    GetProceedure.Name = model.proceeduresForm.Name;
                    GetProceedure.Description = model.proceeduresForm.Description;
                    GetProceedure.ProcedureTypeId = model.proceeduresForm.ProcedureTypeId;
                    GetProceedure.ModifiedBy = User.Identity.Name;
                    GetProceedure.ModifiedDate = DateTime.Now;
                    db.SaveChanges();
                    TempData["message"] = "<b>" + model.proceeduresForm.Name + "</b> was Successfully updated";
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
