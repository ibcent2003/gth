using Project.Areas.Setup.Models;
using Project.DAL;
using Project.Models;
using Project.Properties;
using Project.UI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;



namespace Project.Areas.Setup.Controllers
{
    public class NewsManagementController : Controller
    {
        //
        // GET: /Setup/NewsManagement/
        private PROEntities db = new PROEntities();
        private ProcessUtility util;
        private string filePath;
        public NewsManagementController()
        {
            this.util = new ProcessUtility();
            this.filePath = "~/Content/Backend/News/";
        }
        public ActionResult Index()
        {
            try
            {
                var rowsToShow = db.News.ToList();
                var viewModel = new NewsManagementViewModel
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
                NewsManagementViewModel model = new NewsManagementViewModel();
                model.NewsCategory = (from s in db.NewsCategory select new IntegerSelectListItem { Text = s.Name, Value = s.Id }).ToList();
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

        //      
        [HttpPost, ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public ActionResult Create(NewsManagementViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {


                    var validate = (from m in db.News where m.NewsHeadline == model.newsform.NewsHeadline select m).ToList();
                    if (validate.Any())
                    {
                        model.NewsCategory = (from s in db.NewsCategory select new IntegerSelectListItem { Text = s.Name, Value = s.Id }).ToList();
                        TempData["messageType"] = "danger";
                        TempData["message"] = "The News Headline" + model.newsform.NewsHeadline + " already exist. Please try different headline";
                        return View(model);
                    }
                    #region upload Resume

                    int max_upload = 5242880;

                    string ResumePath = Server.MapPath(filePath);
                  //  List<DocumentInfo> uploadedResume = new List<DocumentInfo>();

                    CodeGenerator CodePassport = new CodeGenerator();
                    string EncKey = util.MD5Hash(DateTime.Now.Ticks.ToString());
                    List<Project.DAL.DocumentFormat> Resumetypes = db.DocumentType.FirstOrDefault(x => x.Id == 1).DocumentFormat.ToList();

                    List<string> supportedResume = new List<string>();
                    foreach (var item in Resumetypes)
                    {
                        supportedResume.Add(item.Extension);
                    }
                    var fileResume = System.IO.Path.GetExtension(model.newsform.Photo.FileName);
                    if (!supportedResume.Contains(fileResume))
                    {
                        model.NewsCategory = (from s in db.NewsCategory select new IntegerSelectListItem { Text = s.Name, Value = s.Id }).ToList();
                        TempData["messageType"] = "danger";
                        TempData["message"] = "Invalid type. Only the following type " + String.Join(",", supportedResume) + " are supported for News Photo ";
                        return View(model);

                    }
                    //else 
                    if (model.newsform.Photo.ContentLength > max_upload)
                    {
                        model.NewsCategory = (from s in db.NewsCategory select new IntegerSelectListItem { Text = s.Name, Value = s.Id }).ToList();
                        TempData["messageType"] = "danger";
                        TempData["message"] = "The News Photo uploaded is larger than the 5MB upload limit";
                        return View(model);
                    }
                    #endregion

                    #region save Resume
                    int i = 0;
                    string filename;
                    filename = EncKey + i.ToString() + System.IO.Path.GetExtension(model.newsform.Photo.FileName);
                    model.newsform.Photo.SaveAs(ResumePath + filename);
                    //uploadedResume.Add(new DocumentInfo { DocumentTypeId = 1, Name = filename, Path = filename, Size = model.newsform.Photo.ContentLength.ToString(), Extension = System.IO.Path.GetExtension(model.newsform.Photo.FileName), ModifiedDate = DateTime.Now, ModifiedBy = User.Identity.Name });
                    #endregion
                    //add to news table 
                    if (System.Web.Security.Roles.GetRolesForUser(User.Identity.Name).Contains("GTH Admin") || System.Web.Security.Roles.GetRolesForUser(User.Identity.Name).Contains("ADMINISTRATOR"))
                    {
                        News add = new News
                        {
                            NewsHeadline = model.newsform.NewsHeadline,
                            NewsContent = model.newsform.NewsContent,
                           // SampleContent = model.newsform.NewsSample,
                            Photo = filename,
                            CreatedBy = User.Identity.Name,
                            NewsCategoryId = model.newsform.NewsCategoryId,
                            CreatedDate = DateTime.Parse(DateTime.Now.ToString("yyyy-MM-dd")),
                            IsDeleted = model.newsform.IsDeleted,
                            IsPublished = model.newsform.IsPublished,
                            ModifiedBy = User.Identity.Name,
                            ModifiedDate = DateTime.Now,
                            NoOfView = 0
                        };
                        db.News.AddObject(add);
                        db.SaveChanges();
                        TempData["message"] = "<b>" + model.newsform.NewsHeadline + "</b> was Successfully created";
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        News add = new News
                        {
                            NewsHeadline = model.newsform.NewsHeadline,
                            NewsContent = model.newsform.NewsContent,
                          //  SampleContent = model.newsform.NewsSample,
                            Photo = filename,
                            CreatedBy = User.Identity.Name,
                            NewsCategoryId = model.newsform.NewsCategoryId,
                            CreatedDate = DateTime.Parse(DateTime.Now.ToString("yyyy-MM-dd")),
                            IsDeleted = model.newsform.IsDeleted,
                            IsPublished = false,
                            ModifiedBy = User.Identity.Name,
                            ModifiedDate = DateTime.Now,
                            NoOfView = 0
                        };
                        db.News.AddObject(add);
                        db.SaveChanges();
                        TempData["message"] = "<b>" + model.newsform.NewsHeadline + "</b> was Successfully created";
                        return RedirectToAction("Index");

                    }
                    

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
                NewsManagementViewModel model = new NewsManagementViewModel();
                model.NewsCategory = (from s in db.NewsCategory select new IntegerSelectListItem { Text = s.Name, Value = s.Id }).ToList();
                var GetNews = db.News.Where(x => x.Id == Id).FirstOrDefault();
                model.news = GetNews;
                model.newsform = new  NewsForm();
                model.newsform.NewsCategoryId = model.news.NewsCategoryId;
                model.newsform.NewsHeadline = model.news.NewsHeadline;              
                model.newsform.NewsContent = model.news.NewsContent;
              //  model.newsform.NewsSample = model.news.SampleContent;
                model.newsform.IsPublished = model.news.IsPublished;
                model.newsform.IsDeleted = model.news.IsDeleted;
                model.newsform.Id = Id;

                model.PicturePath = "/Content/Backend/News/";
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


        [HttpPost, ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(NewsManagementViewModel model)
        {
            try
            {
                var GetNews = db.News.Where(x => x.Id == model.newsform.Id).FirstOrDefault();
                model.NewsCategory = (from s in db.NewsCategory select new IntegerSelectListItem { Text = s.Name, Value = s.Id }).ToList();
                model.news = GetNews;
                if (model.newsform.Photo != null && model.newsform.Photo.ContentLength > 0)
                {
                    #region upload Resume

                    int max_upload = 5242880;

                    string ResumePath = Server.MapPath(this.filePath);
                   // List<DocumentInfo> uploadedResume = new List<DocumentInfo>();

                    CodeGenerator CodePassport = new CodeGenerator();
                    string EncKey = util.MD5Hash(DateTime.Now.Ticks.ToString());
                    List<DocumentFormat> Resumetypes = db.DocumentType.FirstOrDefault(x => x.Id == 1).DocumentFormat.ToList();

                    List<string> supportedResume = new List<string>();
                    foreach (var item in Resumetypes)
                    {
                        supportedResume.Add(item.Extension);
                    }
                    var fileResume = System.IO.Path.GetExtension(model.newsform.Photo.FileName);
                    if (!supportedResume.Contains(fileResume))
                    {
                        TempData["messageType"] = "danger";
                        TempData["message"] = "Invalid type. Only the following type " + String.Join(",", supportedResume) + " are supported for News Photo ";
                        return View(model);

                    }
                    else if (model.newsform.Photo.ContentLength > max_upload)
                    {

                        TempData["messageType"] = "danger";
                        TempData["message"] = "The News Photo uploaded is larger than the 5MB upload limit";
                        return View(model);
                    }
                    #endregion

                    #region save Resume
                    int i = 0;
                    string filename;
                    filename = EncKey + i.ToString() + System.IO.Path.GetExtension(model.newsform.Photo.FileName);
                    model.newsform.Photo.SaveAs(ResumePath + filename);
                    //uploadedResume.Add(new DocumentInfo { DocumentTypeId = 5, Name = filename, Path = filename, Size = model.newsForm.Photo.ContentLength.ToString(), Extension = System.IO.Path.GetExtension(model.newsForm.Photo.FileName), ModifiedDate = DateTime.Now, ModifiedBy = User.Identity.Name, IssuedDate = DateTime.Now });
                    #endregion

                    GetNews.NewsHeadline = model.newsform.NewsHeadline;                   
                    GetNews.NewsContent = model.newsform.NewsContent;
                    //   GetNews.SampleContent = model.newsform.NewsContent;
                    if (System.Web.Security.Roles.GetRolesForUser(User.Identity.Name).Contains("GTH Admin") || System.Web.Security.Roles.GetRolesForUser(User.Identity.Name).Contains("ADMINISTRATOR"))
                    {
                        GetNews.IsPublished = model.newsform.IsPublished;
                    }
                    GetNews.NewsCategoryId = model.newsform.NewsCategoryId;                       
                 //   GetNews.SampleContent = model.newsform.NewsSample;
                    GetNews.Photo = filename;
                    GetNews.ModifiedBy = User.Identity.Name;
                    GetNews.ModifiedDate = DateTime.Now;                   
                    db.SaveChanges();
                    TempData["message"] = "<b>" + model.newsform.NewsHeadline + "</b> was Successfully updated";
                    return RedirectToAction("Index");
                }
                else
                {
                    GetNews.NewsHeadline = model.newsform.NewsHeadline;                   
                    GetNews.NewsContent = model.newsform.NewsContent;
                  //  GetNews.SampleContent = model.newsform.NewsSample;
                    //if (Roles.GetRolesForUser(User.Identity.Name).Contains("GTH Admin"))
                    if(System.Web.Security.Roles.GetRolesForUser(User.Identity.Name).Contains("GTH Admin") || System.Web.Security.Roles.GetRolesForUser(User.Identity.Name).Contains("ADMINISTRATOR"))
                    {
                        GetNews.IsPublished = model.newsform.IsPublished;
                    }
                    GetNews.NewsCategoryId = model.newsform.NewsCategoryId;
                    GetNews.ModifiedBy = User.Identity.Name;
                    GetNews.ModifiedDate = DateTime.Now;                   
                    db.SaveChanges();
                    TempData["message"] = "<b>" + model.newsform.NewsHeadline + "</b> was Successfully updated";
                    return RedirectToAction("Index");
                }
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
