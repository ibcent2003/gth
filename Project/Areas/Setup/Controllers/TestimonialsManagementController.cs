using Elmah;
using Project.Areas.Setup.Models;
using Project.DAL;
using Project.Properties;
using System;
using System.Collections.Generic;
using System.Data.Objects;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Security.Principal;
using System.Web.Mvc;
using System.Web.Security;

namespace Project.Areas.Setup.Controllers
{
    public class TestimonialsManagementController : Controller
    {
        private PROEntities db = new PROEntities();
        //
        // GET: /Setup/TestimonialsManagement/

      

        public ActionResult Create()
        {
            ActionResult action;
            try
            {
                action = base.View(new TestimonialViewModel());
            }
            catch (Exception exception)
            {
                ErrorSignal.FromCurrentContext().Raise(exception);
                base.TempData["messageType"] = "danger";
                base.TempData["message"] = Settings.Default.GenericExceptionMessage;
                action = base.RedirectToAction("Index", "Dashboard", new { area = "Admin" });
            }
            return action;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult Create(TestimonialViewModel model)
        {
            ActionResult action;
            try
            {
                if (!base.ModelState.IsValid)
                {
                    action = base.View(model);
                }
                else if ((
                    from m in this.db.Testimonials
                    where m.Name == model.Testimonialform.Name && m.Message == model.Testimonialform.Content
                    select m).ToList<Testimonials>().Any<Testimonials>())
                {
                    base.TempData["messageType"] = "danger";
                    base.TempData["message"] = string.Concat("The Name", model.Testimonialform.Name, " already exist. Please try different name");
                    action = base.View(model);
                }
                else if (!System.Web.Security.Roles.GetRolesForUser(base.User.Identity.Name).Contains<string>("ADMINISTRATOR"))
                {
                    Testimonials testimonial = new Testimonials()
                    {
                        Name = model.Testimonialform.Name,
                        Message = model.Testimonialform.Content,
                        HasDeleted = model.Testimonialform.IsDeleted,
                        Company = model.Testimonialform.Position,
                        IsPulished = false,
                        ModifiedBy = base.User.Identity.Name,
                        ModifiedDate = DateTime.Now
                    };
                    this.db.Testimonials.AddObject(testimonial);
                    this.db.SaveChanges();
                    base.TempData["message"] = string.Concat("<b>", model.Testimonialform.Name, "</b> was Successfully created");
                    action = base.RedirectToAction("Index");
                }
                else
                {
                    Testimonials testimonial1 = new Testimonials()
                    {
                        Name = model.Testimonialform.Name,
                        Message = model.Testimonialform.Content,
                        HasDeleted = model.Testimonialform.IsDeleted,
                        IsPulished = model.Testimonialform.IsPublished,
                        ModifiedBy = base.User.Identity.Name,
                        Company = model.Testimonialform.Position,
                        ModifiedDate = DateTime.Now
                    };
                    this.db.Testimonials.AddObject(testimonial1);
                    this.db.SaveChanges();
                    base.TempData["message"] = string.Concat("<b>", model.Testimonialform.Name, "</b> was Successfully created");
                    action = base.RedirectToAction("Index");
                }
            }
            catch (Exception exception)
            {
                ErrorSignal.FromCurrentContext().Raise(exception);
                base.TempData["messageType"] = "danger";
                base.TempData["message"] = Settings.Default.GenericExceptionMessage;
                action = base.RedirectToAction("Index", "Dashboard", new { area = "Admin" });
            }
            return action;
        }

        public ActionResult Edit(int Id)
        {
             
            try
            {
                TestimonialViewModel testimonialViewModel = new TestimonialViewModel();
                var testimonial = (
                    from x in this.db.Testimonials
                    where x.Id == Id
                    select x).FirstOrDefault();

                testimonialViewModel.Testimonialform = new TestimonialForm();
                testimonialViewModel.Testimonialform.Name = testimonial.Name;
                testimonialViewModel.Testimonialform.Content = testimonial.Message;
                testimonialViewModel.Testimonialform.IsPublished = testimonial.IsPulished;
                testimonialViewModel.Testimonialform.Position = testimonial.Company;
                testimonialViewModel.Testimonialform.IsDeleted = testimonial.HasDeleted;
                testimonialViewModel.Testimonialform.Id = Id;
                return View(testimonialViewModel);
            }
            catch (Exception exception)
            {
                ErrorSignal.FromCurrentContext().Raise(exception);
                base.TempData["messageType"] = "danger";
                base.TempData["message"] = Settings.Default.GenericExceptionMessage;
                return RedirectToAction("Index", "Home", new { area = "Admin" });
            }
             
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult Edit(TestimonialViewModel model)
        {
            ActionResult action;
            try
            {
                Testimonials name = (
                    from x in this.db.Testimonials
                    where x.Id == model.Testimonialform.Id
                    select x).FirstOrDefault<Testimonials>();
                name.Name = model.Testimonialform.Name;
                name.Message = model.Testimonialform.Content;
                if (System.Web.Security.Roles.GetRolesForUser(base.User.Identity.Name).Contains<string>("ADMINISTRATOR"))
                {
                    name.IsPulished = model.Testimonialform.IsPublished;
                }
                name.HasDeleted = model.Testimonialform.IsDeleted;
                name.ModifiedBy = base.User.Identity.Name;
                name.Company = model.Testimonialform.Position;
                name.ModifiedDate = DateTime.Now;
                this.db.SaveChanges();
                base.TempData["message"] = string.Concat("<b>", model.Testimonialform.Name, "</b> was Successfully updated");
                action = base.RedirectToAction("Index");
            }
            catch (Exception exception)
            {
                ErrorSignal.FromCurrentContext().Raise(exception);
                base.TempData["messageType"] = "danger";
                base.TempData["message"] = Settings.Default.GenericExceptionMessage;
                action = base.RedirectToAction("Index", "Home", new { area = "Admin" });
            }
            return action;
        }

        public ActionResult Index()
        {
            ActionResult action;
            try
            {
                List<Testimonials> list = this.db.Testimonials.ToList<Testimonials>();
                action = base.View(new TestimonialViewModel()
                {
                    Rows = (
                        from x in list
                        orderby x.ModifiedDate descending
                        select x).ToList<Testimonials>()
                });
            }
            catch (Exception exception1)
            {
                Exception exception = exception1;
                base.TempData["messageType"] = "alert-danger";
                base.TempData["message"] = "There is an error in the application. Please try again or contact the system administrator";
                ErrorSignal.FromCurrentContext().Raise(exception);
                action = base.RedirectToAction("Index", "Dashboard", new { area = "Admin" });
            }
            return action;
        }
    }
}
