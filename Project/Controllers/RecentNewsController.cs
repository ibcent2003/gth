using Project.DAL;
using Project.Models;
using Project.Properties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GNSW.DAL;

namespace Project.Controllers
{
    public class RecentNewsController : Controller
    {
        //
        // GET: /RecentNews/
        private PROEntities db = new PROEntities();
        private GNSWEntities swdb = new GNSWEntities();

        public ActionResult Index(RecentNewsViewModel model)
        {
            try
            {

                model.NewsList = db.News.Where(x => x.IsPublished == true && x.IsDeleted == false).ToList();
                model.PicturePath = Settings.Default.PhotoPath;

              //  model.NewsList = (from n in db.News select n).Take(5).ToList();
                var GetNew = (from s in db.News select s).Distinct().ToList();
                List<DateTime> result = GetNew.Select(d => new DateTime(d.CreatedDate.Year, d.CreatedDate.Month, 1)).Distinct().ToList();
                model.newslist = result;


                model.newsCategory = (from n in db.News select n.NewsCategory.Name).Distinct().ToList();


                return View(model);
            }
            catch (Exception ex)
            {
                TempData["messageType"] = "alert-danger";
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                return RedirectToAction("Error404", "Home", new { area = "Admin" });
            }
        }

    }
}
