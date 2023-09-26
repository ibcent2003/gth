using Project.DAL;
using Project.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Project.Controllers
{
    public class HelpController : Controller
    {
        private PROEntities db = new PROEntities();

        public ActionResult Faq(HelpViewModel model)
        {
            try
            {
                model.FAQList = db.FAQ.OrderBy(x => x.Question).ToList();
                return View(model);
            }
            catch(Exception ex)
            {
                TempData["messageType"] = "alert-danger";
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                return RedirectToAction("Error404", "Home", new { area = "" });
            }
            
        }

        public ActionResult CommonlyUsedTerms(HelpViewModel model)
        {
            try
            {
                model.UsedTermList = db.CommonlyUsedTerms.OrderBy(x => x.Terms).ToList();
                return View(model);
            }
            catch (Exception ex)
            {
                TempData["messageType"] = "alert-danger";
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                return RedirectToAction("Error404", "Home", new { area = "" });
            }
        }

        public ActionResult Guideline(HelpViewModel model)
        {
            try
            {
                model.GuidelineList = db.Guideline.Where(x=>x.EndDate == null).OrderBy(x => x.Name).ToList();
                model.DownloadPath = Properties.Settings.Default.DocumentsPath;
                model.MasterUrl = Properties.Settings.Default.MasterUrl;
                return View(model);
            }
            catch(Exception ex)
            {
                TempData["messageType"] = "alert-danger";
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                return RedirectToAction("Error404", "Home", new { area = "" });
            }
        }

        public ActionResult UsefulLinks(HelpViewModel model)
        {
            try
            {
                model.UsefulLink = db.UsefulLink.Where(x => x.EndDate == null).OrderBy(x => x.Name).ToList();
                model.DownloadPath = Properties.Settings.Default.DocumentsPath;
                return View(model);
            }
            catch (Exception ex)
            {
                TempData["messageType"] = "alert-danger";
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                return RedirectToAction("Error404", "Home", new { area = "" });
            }
        }


        public ActionResult TermsAndCondition(HelpViewModel model)
        {
            try
            {
                model.TermsCondition = db.TermsAndConditions.FirstOrDefault();               
                return View(model);
            }
            catch (Exception ex)
            {
                TempData["messageType"] = "alert-danger";
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                return RedirectToAction("Error404", "Home", new { area = "" });
            }
        }


        public ActionResult GNSWUsedTerm(HelpViewModel model)
        {
            try
            {
                model.GNSWTerms = db.TermsOfUsed.Where(x=>x.Id==1).FirstOrDefault();
                return View(model);
            }
            catch (Exception ex)
            {
                TempData["messageType"] = "alert-danger";
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                return RedirectToAction("Error404", "Home", new { area = "" });
            }
        }

        public ActionResult GNSWPolicy(HelpViewModel model)
        {
            try
            {
                model.GNSWTerms = db.TermsOfUsed.Where(x => x.Id == 2).FirstOrDefault();
                return View(model);
            }
            catch (Exception ex)
            {
                TempData["messageType"] = "alert-danger";
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                return RedirectToAction("Error404", "Home", new { area = "" });
            }
        }

    }
}
