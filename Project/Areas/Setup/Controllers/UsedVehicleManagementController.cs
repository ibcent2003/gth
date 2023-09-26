using Project.Areas.Setup.Models;
using Project.Models;
using Project.Properties;
using Project.UI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using sw = GNSW.DAL;


namespace Project.Areas.Setup.Controllers
{
    public class UsedVehicleManagementController : Controller
    {
        private sw.GNSWEntities swdb = new sw.GNSWEntities();
        // GET: /Setup/UsedVehicleManagement/

        public ActionResult Index()
        {
            try
            {
                var rowsToShow = swdb.VehicleReference.OrderBy(x=>x.Name).Distinct().ToList();
                var viewModel = new UsedVehicleManagementViewModel
                {
                    Rows = rowsToShow,
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
                UsedVehicleManagementViewModel model = new UsedVehicleManagementViewModel();
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
        public ActionResult Create(UsedVehicleManagementViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {

                    var validate = (from m in swdb.VehicleReference where m.Name == model.VehicleManagementform.Name select m).ToList();
                    if (validate.Any())
                    {
                        TempData["messageType"] = "danger";
                        TempData["message"] = "The Name" + model.VehicleManagementform.Name + " already exist. Please try different Name";
                        return View(model);
                    }

                    sw.VehicleReference add = new sw.VehicleReference
                    {
                        Name = model.VehicleManagementform.Name,                      
                        ModifiedBy = User.Identity.Name,
                        ModifiedDate = DateTime.Now,
                    };
                    swdb.VehicleReference.AddObject(add);
                    swdb.SaveChanges();
                    TempData["message"] = "<b>" + model.VehicleManagementform.Name + "</b> was Successfully created";
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
                UsedVehicleManagementViewModel model = new UsedVehicleManagementViewModel();
                var GetVehicle = swdb.VehicleReference.Where(x => x.Id == Id).FirstOrDefault();
                model.VehicleManagementform = new  VehicleManagementForm();
                model.VehicleManagementform.Name = GetVehicle.Name;               
                model.VehicleManagementform.Id = Id;
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
        public ActionResult Edit(UsedVehicleManagementViewModel model)
        {
            try
            {
                var GetVehicle = swdb.VehicleReference.Where(x => x.Id == model.VehicleManagementform.Id).FirstOrDefault();

                GetVehicle.Name = model.VehicleManagementform.Name;
                GetVehicle.ModifiedBy = User.Identity.Name;
                GetVehicle.ModifiedDate = DateTime.Now;
                swdb.SaveChanges();
                TempData["message"] = "<b>" + model.VehicleManagementform.Name + "</b> was Successfully updated";
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

        public ActionResult ModelList(int Id = 0)
        {
            try
            {
                var GetVehicleType = swdb.VehicleReference.Where(x => x.Id == Id).FirstOrDefault();
                var rowsToShow = swdb.VehicleModelReference.Where(x => x.MakeId == Id).ToList();
                var viewModel = new UsedVehicleManagementViewModel
                {
                    VModelList = rowsToShow.OrderBy(x => x.ModelName).ToList(),
                };
                viewModel.VehicleReference = GetVehicleType;               
                return View(viewModel);
            }
            catch (Exception ex)
            {
                TempData["messageType"] = "danger";
                TempData["message"] = "There is an error in the application. Please try again or contact the system administrator";
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                return RedirectToAction("Index", "Dashboard", new { area = "Admin" });
            }
        }


        public ActionResult NewModel(int Id = 0)
        {
            try
            {
                UsedVehicleManagementViewModel model = new UsedVehicleManagementViewModel();
                var GetVehcileType = swdb.VehicleReference.Where(x => x.Id == Id).FirstOrDefault();
                model.MakeList = (from s in swdb.VehicleReference where s.Id == Id select new IntegerSelectListItem { Text = s.Name, Value = s.Id }).ToList();
                model.modelform = new  ModelForm();
                model.modelform.MakeId = Id;
                model.VehicleReference = GetVehcileType;              
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
        public ActionResult NewModel(UsedVehicleManagementViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {

                    var validate = (from m in swdb.VehicleModelReference where m.ModelName == model.modelform.ModelName && m.MakeId == model.modelform.MakeId select m).ToList();
                    if (validate.Any())
                    {

                        var GetVehcileType = swdb.VehicleReference.Where(x => x.Id == model.VehicleId).FirstOrDefault();
                        model.MakeList = (from s in swdb.VehicleReference where s.Id == model.VehicleId select new IntegerSelectListItem { Text = s.Name, Value = s.Id }).ToList();
                        model.modelform = new ModelForm();
                        model.modelform.MakeId = model.VehicleId;
                        model.VehicleReference = GetVehcileType;
                        TempData["messageType"] = "danger";
                        TempData["message"] = "The Name" + model.modelform.ModelName + " already exist. Please try different Name";
                        return View(model);
                    }

                    sw.VehicleModelReference add = new sw.VehicleModelReference
                    {
                        ModelName = model.modelform.ModelName,
                        MakeId = model.VehicleId,                    
                        ModifiedBy = User.Identity.Name,
                        ModifiedDate = DateTime.Now,
                    };
                    swdb.VehicleModelReference.AddObject(add);
                    swdb.SaveChanges();
                    TempData["message"] = "<b>" + model.modelform.ModelName + "</b> was Successfully created";
                    return RedirectToAction("ModelList", new { Id = model.modelform.MakeId });
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




        public ActionResult EditModel(int Id, int MakeId)
        {
            try
            {
                UsedVehicleManagementViewModel model = new UsedVehicleManagementViewModel();

                var GetMake = swdb.VehicleReference.Where(x => x.Id ==MakeId).FirstOrDefault();

                var GetModel = swdb.VehicleModelReference.Where(x => x.Id == Id && x.MakeId == MakeId).FirstOrDefault();
                model.MakeList = (from s in swdb.VehicleReference where s.Id==MakeId select new IntegerSelectListItem { Text = s.Name, Value = s.Id }).ToList();
                model.modelform = new Models.ModelForm();
                model.modelform.ModelName = GetModel.ModelName;               
                model.modelform.Id = Id;
                model.modelform.MakeId = MakeId;             
                model.VehicleReference = GetMake;               
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
        public ActionResult EditModel(UsedVehicleManagementViewModel model)
        {
            try
            {
                var Getmodel = swdb.VehicleModelReference.Where(x => x.Id == model.modelform.Id && x.MakeId == model.modelform.MakeId).FirstOrDefault();
                Getmodel.ModelName = model.modelform.ModelName;
                Getmodel.ModifiedBy = User.Identity.Name;
                Getmodel.ModifiedDate = DateTime.Now;
                swdb.SaveChanges();
                TempData["message"] = "<b>" + model.modelform.ModelName + "</b> was Successfully updated";
                return RedirectToAction("ModelList", new { Id = model.modelform.MakeId });

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
