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
    public class GuidelinesManagementController : Controller
    {
        private PROEntities db = new PROEntities();
        private ProcessUtility util;
        private string filePath;

        public GuidelinesManagementController()
        {
            this.util = new ProcessUtility();
            this.filePath = "~/Content/Frontend/light/img/Sections/Documents/";
        }

        public ActionResult Index()
        {
            try
            {
                var rowsToShow = db.Guideline.ToList();
                var viewModel = new GuidelineViewModel
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
                GuidelineViewModel model = new GuidelineViewModel();               
                return View(model);
            }
            catch(Exception ex)
            {
                TempData["messageType"] = "alert-danger";
                TempData["message"] = "There is an error in the application. Please try again or contact the system administrator";
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                return RedirectToAction("Index", "Dashboard", new { area = "Admin" });
            }
        }


        [HttpPost, ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public ActionResult Create(GuidelineViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {

                    var validate = (from m in db.Guideline where m.Name == model.guidelineform.Name select m).ToList();
                    if (validate.Any())
                    {                      
                        TempData["messageType"] = "danger";
                        TempData["message"] = "The Name" + model.guidelineform.Name + " already exist. Please try different Name";
                        return View(model);
                    }
                    #region upload document

                    int max_upload = 5242880;

                    string ResumePath = Server.MapPath(filePath);
                    //  List<DocumentInfo> uploadedResume = new List<DocumentInfo>();

                    CodeGenerator CodePassport = new CodeGenerator();
                    string EncKey = util.MD5Hash(DateTime.Now.Ticks.ToString());
                    List<Project.DAL.DocumentFormat> Resumetypes = db.DocumentType.FirstOrDefault(x => x.Id == 2).DocumentFormat.ToList();

                    List<string> supportedResume = new List<string>();
                    foreach (var item in Resumetypes)
                    {
                        supportedResume.Add(item.Extension);
                    }
                    var fileResume = System.IO.Path.GetExtension(model.guidelineform.document.FileName);
                    if (!supportedResume.Contains(fileResume.ToLower()))
                    {                       
                        TempData["messageType"] = "danger";
                        TempData["message"] = "Invalid type. Only the following type " + String.Join(",", supportedResume) + " are supported for document ";
                        return View(model);

                    }
                    //else 
                    if (model.guidelineform.document.ContentLength > max_upload)
                    {                      
                        TempData["messageType"] = "danger";
                        TempData["message"] = "The document uploaded is larger than the 5MB upload limit";
                        return View(model);
                    }
                    #endregion

                    #region save Resume
                    int i = 0;
                    string filename;
                    filename = EncKey + i.ToString() + System.IO.Path.GetExtension(model.guidelineform.document.FileName);
                    model.guidelineform.document.SaveAs(ResumePath + filename);                   
                    #endregion                                     
                        Guideline add = new Guideline
                        {
                            Name = model.guidelineform.Name,
                            Description = model.guidelineform.Description,                           
                            FileName = filename,                           
                            ModifiedBy = User.Identity.Name,
                            ModifiedDate = DateTime.Now,                          
                        };
                        db.Guideline.AddObject(add);
                        db.SaveChanges();
                        TempData["message"] = "<b>" + model.guidelineform.Name + "</b> was Successfully created";
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
                GuidelineViewModel model = new GuidelineViewModel();              
                var GetGuideline = db.Guideline.Where(x => x.Id == Id).FirstOrDefault();                
                model.guidelineform = new  GuidelineForm();
                model.guidelineform.Name = GetGuideline.Name;
                model.guidelineform.Description = GetGuideline.Description;              
                model.guidelineform.Id = Id;

                model.PicturePath = "/Content/Frontend/light/img/Sections/Documents/"+GetGuideline.FileName;
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
        public ActionResult Edit(GuidelineViewModel model)
        {
            try
            {
                var GetGuideline = db.Guideline.Where(x => x.Id == model.guidelineform.Id).FirstOrDefault();
               
              
                if (model.guidelineform.document != null && model.guidelineform.document.ContentLength > 0)
                {
                    #region upload Resume

                    int max_upload = 5242880;

                    string ResumePath = Server.MapPath(this.filePath);
                   
                    CodeGenerator CodePassport = new CodeGenerator();
                    string EncKey = util.MD5Hash(DateTime.Now.Ticks.ToString());
                    List<DocumentFormat> Resumetypes = db.DocumentType.FirstOrDefault(x => x.Id == 2).DocumentFormat.ToList();

                    List<string> supportedResume = new List<string>();
                    foreach (var item in Resumetypes)
                    {
                        supportedResume.Add(item.Extension);
                    }
                    var fileResume = System.IO.Path.GetExtension(model.guidelineform.document.FileName);
                    if (!supportedResume.Contains(fileResume.ToLower()))
                    {
                        TempData["messageType"] = "danger";
                        TempData["message"] = "Invalid type. Only the following type " + String.Join(",", supportedResume) + " are supported for document ";
                        return View(model);

                    }
                    else if (model.guidelineform.document.ContentLength > max_upload)
                    {

                        TempData["messageType"] = "danger";
                        TempData["message"] = "The document uploaded is larger than the 5MB upload limit";
                        return View(model);
                    }
                    #endregion

                    #region save Resume
                    int i = 0;
                    string filename;
                    filename = EncKey + i.ToString() + System.IO.Path.GetExtension(model.guidelineform.document.FileName);
                    model.guidelineform.document.SaveAs(ResumePath + filename);

                    #endregion

                    GetGuideline.Name = model.guidelineform.Name;
                    GetGuideline.Description = model.guidelineform.Description;
                    GetGuideline.FileName = filename;
                    GetGuideline.ModifiedBy = User.Identity.Name;
                    GetGuideline.ModifiedDate = DateTime.Now;
                    db.SaveChanges();
                    TempData["message"] = "<b>" + model.guidelineform.Name + "</b> was Successfully updated";
                    return RedirectToAction("Index");
                }
                else
                {
                    GetGuideline.Name = model.guidelineform.Name;
                    GetGuideline.Description = model.guidelineform.Description;
                    GetGuideline.ModifiedBy = User.Identity.Name;
                    GetGuideline.ModifiedDate = DateTime.Now;
                    db.SaveChanges();
                    TempData["message"] = "<b>" + model.guidelineform.Name + "</b> was Successfully updated";
                    return RedirectToAction("Index");
                }
            }
            catch(Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                TempData["messageType"] = "danger";
                TempData["message"] = Settings.Default.GenericExceptionMessage;
                return RedirectToAction("Index", "Home", new { area = "Admin" });
            }
        }

    }
}
