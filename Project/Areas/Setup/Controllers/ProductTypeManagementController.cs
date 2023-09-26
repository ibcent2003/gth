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
    public class ProductTypeManagementController : Controller
    {
        //
        // GET: /Setup/ProductTypeManagement/
        private PROEntities db = new PROEntities();
        private ProcessUtility util;
        private string filePath;

        public ProductTypeManagementController()
        {
            this.util = new ProcessUtility();
            this.filePath = "~/Content/Frontend/light/img/sections/MarketHub/";
        }


        public ActionResult Index()
        {
            try
            {
                var rowsToShow = db.ProductType.ToList();
                var viewModel = new ProductTypeViewModel
                {
                    Rows = rowsToShow.OrderBy(x => x.Name).ToList(),
                };
                viewModel.PicturePath = "/Content/Frontend/light/img/sections/MarketHub/";
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
                ProductTypeViewModel model = new ProductTypeViewModel();             
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


        [HttpPost, ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public ActionResult Create(ProductTypeViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {

                    var validate = (from m in db.ProductType where m.Name == model.producttypeform.Name select m).ToList();
                    if (validate.Any())
                    {                       
                        TempData["messageType"] = "alert-danger";
                        TempData["message"] = "The Name" + model.producttypeform.Name + " already exist. Please try different Name";
                        return View(model);
                    }
                    #region upload document

                    int max_upload = 5242880;

                    string ResumePath = Server.MapPath(filePath);

                    CodeGenerator CodePassport = new CodeGenerator();
                    string EncKey = util.MD5Hash(DateTime.Now.Ticks.ToString());
                    List<Project.DAL.DocumentFormat> Resumetypes = db.DocumentType.FirstOrDefault(x => x.Id == 1).DocumentFormat.ToList();

                    List<string> supportedResume = new List<string>();
                    foreach (var item in Resumetypes)
                    {
                        supportedResume.Add(item.Extension);
                    }
                    var fileResume = System.IO.Path.GetExtension(model.producttypeform.Photo.FileName);
                    if (!supportedResume.Contains(fileResume.ToLower()))
                    {                       
                        TempData["messageType"] = "danger";
                        TempData["message"] = "Invalid type. Only the following type " + String.Join(",", supportedResume) + " are supported for Product Photo ";
                        return View(model);

                    }
                    //else 
                    if (model.producttypeform.Photo.ContentLength > max_upload)
                    {                     
                        TempData["messageType"] = "danger";
                        TempData["message"] = "The Photo uploaded is larger than the 5MB upload limit";
                        return View(model);
                    }
                    #endregion

                    #region save Resume
                    int i = 0;
                    string filename;
                    filename = EncKey + i.ToString() + System.IO.Path.GetExtension(model.producttypeform.Photo.FileName);
                    model.producttypeform.Photo.SaveAs(ResumePath + filename);
                    #endregion
                    ProductType add = new ProductType
                    {
                        Name = model.producttypeform.Name,
                        Photo = filename,
                        Description = model.producttypeform.Description,                      
                        ModifiedBy = User.Identity.Name,
                        ModifiedDate = DateTime.Now,
                    };
                    db.ProductType.AddObject(add);
                    db.SaveChanges();
                    TempData["message"] = "<b>" + model.producttypeform.Name + "</b> was Successfully created";
                    return RedirectToAction("Index");

                }               
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
                ProductTypeViewModel model = new ProductTypeViewModel();
                var GetProductType = db.ProductType.Where(x => x.Id == Id).FirstOrDefault();               
                model.producttypeform = new Models.ProductTypeForm();
                model.producttypeform.Name = GetProductType.Name;            
                model.producttypeform.Description = GetProductType.Description;               
                model.producttypeform.Id = Id;
                model.producttypeform.picture = GetProductType.Photo;
                model.PicturePath = "/Content/Frontend/light/img/sections/MarketHub/";
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


        [HttpPost, ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(ProductTypeViewModel model)
        {
            try
            {
                var GetProductType = db.ProductType.Where(x => x.Id == model.producttypeform.Id).FirstOrDefault();
                if (model.producttypeform.Photo != null && model.producttypeform.Photo.ContentLength > 0)
                {
                    if (GetProductType.Photo != null)
                    {
                        System.IO.FileInfo fi = new System.IO.FileInfo(Properties.Settings.Default.DocPath + GetProductType.Photo);
                        fi.Delete();
                    }
                    #region upload document

                    int max_upload = 5242880;

                    string ResumePath = Server.MapPath(filePath);

                    CodeGenerator CodePassport = new CodeGenerator();
                    string EncKey = util.MD5Hash(DateTime.Now.Ticks.ToString());
                    List<Project.DAL.DocumentFormat> Resumetypes = db.DocumentType.FirstOrDefault(x => x.Id == 1).DocumentFormat.ToList();

                    List<string> supportedResume = new List<string>();
                    foreach (var item in Resumetypes)
                    {
                        supportedResume.Add(item.Extension);
                    }
                    var fileResume = System.IO.Path.GetExtension(model.producttypeform.Photo.FileName);
                    if (!supportedResume.Contains(fileResume.ToLower()))
                    {                      
                        TempData["messageType"] = "danger";
                        TempData["message"] = "Invalid type. Only the following type " + String.Join(",", supportedResume) + " are supported for Photo ";
                        return View(model);

                    }
                    //else 
                    if (model.producttypeform.Photo.ContentLength > max_upload)
                    {                        
                        TempData["messageType"] = "danger";
                        TempData["message"] = "The Photo uploaded is larger than the 5MB upload limit";
                        return View(model);
                    }
                    #endregion

                    #region save Resume
                    int i = 0;
                    string filename;
                    filename = EncKey + i.ToString() + System.IO.Path.GetExtension(model.producttypeform.Photo.FileName);
                    model.producttypeform.Photo.SaveAs(ResumePath + filename);
                    #endregion
                    GetProductType.Name = model.producttypeform.Name;
                    GetProductType.Description = model.producttypeform.Description;                   
                    GetProductType.Photo = filename;
                    GetProductType.ModifiedBy = User.Identity.Name;
                    GetProductType.ModifiedDate = DateTime.Now;
                    db.SaveChanges();
                    TempData["message"] = "<b>" + model.producttypeform.Name + "</b> was Successfully updated";
                    return RedirectToAction("Index");
                }
                else
                {
                    GetProductType.Name = model.producttypeform.Name;
                    GetProductType.Description = model.producttypeform.Description;                  
                    GetProductType.ModifiedBy = User.Identity.Name;
                    GetProductType.ModifiedDate = DateTime.Now;
                    db.SaveChanges();
                    TempData["message"] = "<b>" + model.producttypeform.Name + "</b> was Successfully updated";
                    return RedirectToAction("Index");
                }            
            }
            catch (Exception ex)
            {
                TempData["messageType"] = "alert-danger";
                TempData["message"] = "There is an error in the application. Please try again or contact the system administrator";
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                return RedirectToAction("Index", "Dashboard", new { area = "Admin" });
            }
        }


        public ActionResult ProductList(int Id=0)
        {
            try
            {
                var GetProductType = db.ProductType.Where(x => x.Id == Id).FirstOrDefault();
                var rowsToShow = db.Product.Where(x=>x.ProductTypeId==Id).ToList();
                var viewModel = new ProductTypeViewModel
                {
                    products = rowsToShow.OrderBy(x => x.Name).ToList(),
                };
                viewModel.ProductType = GetProductType;
                viewModel.PicturePath = "/Content/Frontend/light/img/sections/MarketHub/";
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


        public ActionResult NewProduct(int Id=0)
        {
            try
            {
                ProductTypeViewModel model = new ProductTypeViewModel();
                var GetProductType = db.ProductType.Where(x => x.Id == Id).FirstOrDefault();
                model.ProductTypeList = (from s in db.ProductType where s.Id==Id select new IntegerSelectListItem { Text = s.Name, Value = s.Id }).ToList();
                model.productform = new ProductForm();
                model.productform.ProductTypeId = Id;

                model.ProductType = GetProductType;
                model.PicturePath = "/Content/Frontend/light/img/sections/MarketHub/";
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
        public ActionResult NewProduct(ProductTypeViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {

                    var validate = (from m in db.Product where m.Name == model.productform.Name && m.ProductTypeId==model.productform.ProductTypeId select m).ToList();
                    if (validate.Any())
                    {
                        model.ProductTypeList = (from s in db.ProductType where s.Id == model.productform.ProductTypeId select new IntegerSelectListItem { Text = s.Name, Value = s.Id }).ToList();
                        TempData["messageType"] = "alert-danger";
                        TempData["message"] = "The Name" + model.productform.Name + " already exist. Please try different Name";
                        return View(model);
                    }
                  
                    Product add = new Product
                    {
                        Name = model.productform.Name,
                        ProductTypeId = model.productform.ProductTypeId,
                        Description = model.productform.Description,
                        ModifiedBy = User.Identity.Name,
                        ModifiedDate = DateTime.Now,
                    };
                    db.Product.AddObject(add);
                    db.SaveChanges();
                    TempData["message"] = "<b>" + model.productform.Name + "</b> was Successfully created";
                    return RedirectToAction("ProductList", new { Id = model.productform.ProductTypeId });
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


        public ActionResult EditProduct(int Id, int ProductTypeId)
        {
            try
            {
                ProductTypeViewModel model = new ProductTypeViewModel();

                var GetProductType = db.ProductType.Where(x => x.Id == ProductTypeId).FirstOrDefault();

                var GetProduct = db.Product.Where(x => x.Id == Id && x.ProductTypeId==ProductTypeId).FirstOrDefault();
                model.ProductTypeList = (from s in db.ProductType select new IntegerSelectListItem { Text = s.Name, Value = s.Id }).ToList();
                model.productform = new Models.ProductForm();
                model.productform.Name = GetProduct.Name;
                model.productform.Description = GetProduct.Description;
                model.productform.Id = Id;
                model.productform.ProductTypeId = ProductTypeId;
              //  model.ProductType = GetProductType;
                model.ProductType = GetProductType;
                model.PicturePath = "/Content/Frontend/light/img/sections/MarketHub/";
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



        [HttpPost, ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public ActionResult EditProduct(ProductTypeViewModel model)
        {
            try
            {
                    var GetProduct = db.Product.Where(x => x.Id ==model.productform.Id && x.ProductTypeId == model.productform.ProductTypeId).FirstOrDefault();
                    GetProduct.Name = model.productform.Name;
                    GetProduct.Description = model.productform.Description;
                    GetProduct.ModifiedBy = User.Identity.Name;
                    GetProduct.ModifiedDate = DateTime.Now;
                    db.SaveChanges();
                    TempData["message"] = "<b>" + model.productform.Name + "</b> was Successfully updated";
                    return RedirectToAction("ProductList", new { Id = model.productform.ProductTypeId });

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
