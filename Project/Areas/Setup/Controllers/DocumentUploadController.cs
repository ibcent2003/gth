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
    public class DocumentUploadController : Controller
    {
        private PROEntities db = new PROEntities();
        private ProcessUtility util;
        private string filePath;

        public DocumentUploadController()
        {
            this.util = new ProcessUtility();
            this.filePath = "~/Content/Frontend/light/img/sections/Documents/";
        }
        public ActionResult Index()
        {
            try
            {
                var rowsToShow = db.DocumentInfo.ToList();
                var viewModel = new  UploadDocumentViewModel
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
                UploadDocumentViewModel model = new UploadDocumentViewModel();
                model.DocumentCategoryList = (from s in db.DocumentCategory select new IntegerSelectListItem { Text = s.Name, Value = s.Id }).ToList();
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
        public ActionResult Create(UploadDocumentViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {

                    var validate = (from m in db.DocumentInfo where m.Name == model.documentform.Name select m).ToList();
                    if (validate.Any())
                    {
                        model.DocumentCategoryList = (from s in db.DocumentCategory select new IntegerSelectListItem { Text = s.Name, Value = s.Id }).ToList();
                        TempData["messageType"] = "alert-danger";
                        TempData["message"] = "The Name" + model.documentform.Name + " already exist. Please try different Name";
                        return View(model);
                    }
                    #region upload document

                    int max_upload = 5242880;

                    string ResumePath = Server.MapPath(filePath);                   

                    CodeGenerator CodePassport = new CodeGenerator();
                    string EncKey = util.MD5Hash(DateTime.Now.Ticks.ToString());
                    List<Project.DAL.DocumentFormat> Resumetypes = db.DocumentType.FirstOrDefault(x => x.Id == 2).DocumentFormat.ToList();

                    List<string> supportedResume = new List<string>();
                    foreach (var item in Resumetypes)
                    {
                        supportedResume.Add(item.Extension);
                    }
                    var fileResume = System.IO.Path.GetExtension(model.documentform.document.FileName);
                    if (!supportedResume.Contains(fileResume.ToLower()))
                    {
                        model.DocumentCategoryList = (from s in db.DocumentCategory select new IntegerSelectListItem { Text = s.Name, Value = s.Id }).ToList();
                        TempData["messageType"] = "danger";
                        TempData["message"] = "Invalid type. Only the following type " + String.Join(",", supportedResume) + " are supported for document ";
                        return View(model);

                    }
                    //else 
                    if (model.documentform.document.ContentLength > max_upload)
                    {
                        model.DocumentCategoryList = (from s in db.DocumentCategory select new IntegerSelectListItem { Text = s.Name, Value = s.Id }).ToList();
                        TempData["messageType"] = "danger";
                        TempData["message"] = "The document uploaded is larger than the 5MB upload limit";
                        return View(model);
                    }
                    #endregion

                    #region save Resume
                    int i = 0;
                    string filename;
                    filename = EncKey + i.ToString() + System.IO.Path.GetExtension(model.documentform.document.FileName);
                    model.documentform.document.SaveAs(ResumePath + filename);
                    #endregion
                    DocumentInfo add = new DocumentInfo
                    {
                        Name = model.documentform.Name,
                        Path = filename,
                        Size = model.documentform.document.ContentLength.ToString(),
                        Extension = System.IO.Path.GetExtension(model.documentform.document.FileName),
                        DocumentTypeId=7,
                        IssuedDate = DateTime.Now,
                        DocumentCategoryId = model.documentform.DocumentCategoryId,
                        ExtentalLink = model.documentform.ExtentalLink,
                        WebSiteLink = model.documentform.WebSiteLink,
                        ModifiedBy = User.Identity.Name,
                        ModifiedDate = DateTime.Now,
                    };
                    db.DocumentInfo.AddObject(add);
                    db.SaveChanges();
                    TempData["message"] = "<b>" + model.documentform.Name + "</b> was Successfully created";
                    return RedirectToAction("Index");
                }
                model.DocumentCategoryList = (from s in db.DocumentCategory select new IntegerSelectListItem { Text = s.Name, Value = s.Id }).ToList();
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
                UploadDocumentViewModel model = new UploadDocumentViewModel();
                var GetDocuments = db.DocumentInfo.Where(x => x.Id == Id).FirstOrDefault();
                model.DocumentCategoryList = (from s in db.DocumentCategory select new IntegerSelectListItem { Text = s.Name, Value = s.Id }).ToList();
                model.documentform = new Models.DocumentForm();
                model.documentform.Name = GetDocuments.Name;
                model.documentform.DocumentCategoryId = GetDocuments.DocumentCategoryId;
                model.documentform.ExtentalLink = GetDocuments.ExtentalLink;
                model.documentform.WebSiteLink = GetDocuments.WebSiteLink;
                model.documentform.Id = Id;
                model.path = GetDocuments.Path;
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
        public ActionResult Edit(UploadDocumentViewModel model)
        {
            try
            {
                var GetDocuments = db.DocumentInfo.Where(x => x.Id == model.documentform.Id).FirstOrDefault();
                if (model.documentform.document != null && model.documentform.document.ContentLength > 0)
                {
                    if(GetDocuments.Path != null)
                    {                        
                     System.IO.FileInfo fi = new System.IO.FileInfo(Properties.Settings.Default.DocPath + GetDocuments.Path);
                     fi.Delete();
                    }
                    #region upload document

                    int max_upload = 5242880;

                    string ResumePath = Server.MapPath(filePath);

                    CodeGenerator CodePassport = new CodeGenerator();
                    string EncKey = util.MD5Hash(DateTime.Now.Ticks.ToString());
                    List<Project.DAL.DocumentFormat> Resumetypes = db.DocumentType.FirstOrDefault(x => x.Id == 2).DocumentFormat.ToList();

                    List<string> supportedResume = new List<string>();
                    foreach (var item in Resumetypes)
                    {
                        supportedResume.Add(item.Extension);
                    }
                    var fileResume = System.IO.Path.GetExtension(model.documentform.document.FileName);
                    if (!supportedResume.Contains(fileResume.ToLower()))
                    {
                        model.DocumentCategoryList = (from s in db.DocumentCategory select new IntegerSelectListItem { Text = s.Name, Value = s.Id }).ToList();
                        TempData["messageType"] = "danger";
                        TempData["message"] = "Invalid type. Only the following type " + String.Join(",", supportedResume) + " are supported for document ";
                        return View(model);

                    }
                    //else 
                    if (model.documentform.document.ContentLength > max_upload)
                    {
                        model.DocumentCategoryList = (from s in db.DocumentCategory select new IntegerSelectListItem { Text = s.Name, Value = s.Id }).ToList();
                        TempData["messageType"] = "danger";
                        TempData["message"] = "The document uploaded is larger than the 5MB upload limit";
                        return View(model);
                    }
                    #endregion

                    #region save Resume
                    int i = 0;
                    string filename;
                    filename = EncKey + i.ToString() + System.IO.Path.GetExtension(model.documentform.document.FileName);
                    model.documentform.document.SaveAs(ResumePath + filename);
                    #endregion
                    GetDocuments.Name = model.documentform.Name;
                    GetDocuments.DocumentCategoryId = model.documentform.DocumentCategoryId;
                    GetDocuments.ExtentalLink = model.documentform.ExtentalLink;
                    GetDocuments.WebSiteLink = model.documentform.WebSiteLink;
                    GetDocuments.Path = filename;
                    GetDocuments.Size = model.documentform.document.ContentLength.ToString();
                    GetDocuments.Extension = System.IO.Path.GetExtension(model.documentform.document.FileName);
                    GetDocuments.ModifiedBy = User.Identity.Name;
                    GetDocuments.ModifiedDate = DateTime.Now;
                    db.SaveChanges();
                    TempData["message"] = "<b>" + model.documentform.Name + "</b> was Successfully updated";
                    return RedirectToAction("Index");
                }
                else
                {
                    GetDocuments.Name = model.documentform.Name;
                    GetDocuments.DocumentCategoryId = model.documentform.DocumentCategoryId;
                    GetDocuments.ExtentalLink = model.documentform.ExtentalLink;
                    GetDocuments.WebSiteLink = model.documentform.WebSiteLink;                   
                    GetDocuments.ModifiedBy = User.Identity.Name;
                    GetDocuments.ModifiedDate = DateTime.Now;
                    db.SaveChanges();
                    TempData["message"] = "<b>" + model.documentform.Name + "</b> was Successfully updated";
                    return RedirectToAction("Index");
                }
                 //   return View(model);
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
