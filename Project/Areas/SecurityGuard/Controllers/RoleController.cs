using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using System.Web.Security;
using SecurityGuard.Services;
using SecurityGuard.Interfaces;
using SecurityGuard.ViewModels;
using Project.Controllers;
using Project.DAL;
using System.ComponentModel;
using Project.Properties;
using Project.UI.Models;
using sw = GNSW.DAL;
using System.Configuration;
using Project.Models;

namespace Project.UI.Areas.SecurityGuard.Controllers
{
    [Authorize(Roles = "ADMINISTRATOR, Organisation Admin, PAAR MANAGEMENT, CHA ADMIN")]
    public partial class RoleController : BaseController
    {

        #region ctors

        private readonly IRoleService roleService;
        private PROEntities db = new PROEntities();
        private sw.GNSWEntities swdb = new sw.GNSWEntities();
     //   private Crud crud = new Crud();

        public RoleController()
        {
            this.roleService = new RoleService(System.Web.Security.Roles.Provider);
           // this.db = this.crud.db;
        }

        #endregion


        public virtual ActionResult Index([DefaultValue(0)] int Id)
        {
            try
            {
                sw.Organisation organisation;
                ManageRolesViewModel model = new ManageRolesViewModel();
                var systemCode = ConfigurationManager.AppSettings["SystemCode"].ToString();
                var systemObj = swdb.Systems.FirstOrDefault(x => x.Code == systemCode);
                if (System.Web.Security.Roles.IsUserInRole(Settings.Default.OrganisationAdminRole) && !System.Web.Security.Roles.IsUserInRole("ADMINISTRATOR"))
                {
                    GetOrganisation objGetOrganisation = new GetOrganisation();
                    MembershipUser user = Membership.GetUser(User.Identity.Name);
                    organisation = objGetOrganisation.byUser(user);
                    if (organisation == null)
                        throw new Exception("Could not find Organisation");
                        Id = organisation.Id;
                }

                if (Id != 0)
                {
                    organisation = (swdb.Organisation.Where(x => x.Id == Id)).FirstOrDefault();
                    if (organisation == null)
                        throw new Exception("Could not find Organisation");
                    var roles = (organisation.Roles.Select(x => x.RoleName)).ToArray();
                    model.Roles = new SelectList(roles);
                    model.RoleList = roles;
                    model.objOrganisation = organisation;
                    model.AvailableRoles = new SelectList(systemObj.Roles.Select(x=>x.RoleName).Except(roles));
                    //model.Id = Id;
                }
                else
                {
                    model.Roles = new SelectList(systemObj.Roles.Select(x => x.RoleName));
                    model.RoleList = systemObj.Roles.Select(x => x.Description).ToArray();
                }
                return View(model);
            }
            catch (Exception ex)
            {
                // Log with Elmah
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                TempData["message"] = Settings.Default.GenericExceptionMessage;
                return RedirectToAction("Index", "Home", new { area = "Admin" });
            }
        }

        #region Create Roles Methods
        [Authorize(Roles = "ADMINISTRATOR")]
        [HttpGet]
        public virtual ActionResult CreateRole()
        {
            return View(new RoleViewModel());
        }

        [Authorize(Roles = "ADMINISTRATOR")]
        [HttpPost]
        public virtual ActionResult CreateRole(string roleName, [DefaultValue(0)] int Id)
        {
            JsonResponse response = new JsonResponse();
            if (string.IsNullOrEmpty(roleName))
            {
                response.Success = false;
                response.Message = "You must enter a role name.";
                response.CssClass = "red";

                return Json(response);
            }

            try
            {
                sw.Organisation organisation;
                roleService.CreateRole(roleName);
                if (System.Web.Security.Roles.IsUserInRole(Settings.Default.OrganisationAdminRole) && !System.Web.Security.Roles.IsUserInRole("ADMINISTRATOR"))
                {
                    GetOrganisation objGetOrganisation = new GetOrganisation();
                    MembershipUser user = Membership.GetUser(User.Identity.Name);
                    organisation = objGetOrganisation.byUser(user);
                    if (organisation == null)
                        throw new Exception("Could not find Organisation");
                        Id = organisation.Id;
                }

                if (Request.IsAjaxRequest())
                {
                    if (Id != 0)
                    {
                        db.ContextOptions.LazyLoadingEnabled = false;
                        organisation = (swdb.Organisation.Include("Roles").Where(x => x.Id == Id)).FirstOrDefault();
                        if (organisation == null)
                            throw new Exception("Could not find Organisation");
                        var role = (from r in swdb.Roles where r.RoleName == roleName select r).FirstOrDefault();
                        //organisation.Roles.Remove(role);
                        organisation.Roles.Add(role);
                        swdb.SaveChanges();
                    }
                    response.Success = true;
                    response.Message = "Role created successfully!";
                    response.CssClass = "green";

                    return Json(response);
                }

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                if (Request.IsAjaxRequest())
                {
                    response.Success = false;
                    response.Message = ex.Message;
                    response.CssClass = "red";

                    return Json(response);
                }

                ModelState.AddModelError("", ex.Message);
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        public virtual ActionResult AttachRole(string roleName, [DefaultValue(0)] int Id)
        {
            JsonResponse response = new JsonResponse();
            if (string.IsNullOrEmpty(roleName))
            {
                response.Success = false;
                response.Message = "You must select a role name.";
                response.CssClass = "red";

                return Json(response);
            }

            try
            {
                sw.Organisation organisation;
                //roleService.CreateRole(roleName);
                if (System.Web.Security.Roles.IsUserInRole(Settings.Default.OrganisationAdminRole) && !System.Web.Security.Roles.IsUserInRole("ADMINISTRATOR"))
                {
                    GetOrganisation objGetOrganisation = new GetOrganisation();
                    MembershipUser user = Membership.GetUser(User.Identity.Name);
                    organisation = objGetOrganisation.byUser(user);
                    if (organisation == null)
                        throw new Exception("Could not find Organisation");
                    Id = organisation.Id;
                }

                if (Request.IsAjaxRequest())
                {
                    if (Id != 0)
                    {
                        swdb.ContextOptions.LazyLoadingEnabled = false;
                        organisation = (swdb.Organisation.Include("Roles").Where(x => x.Id == Id)).FirstOrDefault();
                        if (organisation == null)
                            throw new Exception("Could not find Organisation");
                        var role = (from r in swdb.Roles where r.RoleName == roleName select r).FirstOrDefault();
                        //organisation.Roles.Remove(role);
                        organisation.Roles.Add(role);
                        swdb.SaveChanges();
                    }
                    response.Success = true;
                    response.Message = "Role attached successfully!";
                    response.CssClass = "green";

                    return Json(response);
                }

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                if (Request.IsAjaxRequest())
                {
                    response.Success = false;
                    response.Message = ex.Message;
                    response.CssClass = "red";

                    return Json(response);
                }

                ModelState.AddModelError("", ex.Message);
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        public virtual ActionResult RemoveRole(string roleName, [DefaultValue(0)] int Id)
        {
            JsonResponse response = new JsonResponse();
            if (string.IsNullOrEmpty(roleName))
            {
                response.Success = false;
                response.Message = "You must select a role to remove.";
                response.CssClass = "red";

                return Json(response);
            }

            try
            {
                sw.Organisation organisation;
                //roleService.CreateRole(roleName);
                if (System.Web.Security.Roles.IsUserInRole(Settings.Default.OrganisationAdminRole) && !System.Web.Security.Roles.IsUserInRole("ADMINISTRATOR"))
                {
                    GetOrganisation objGetOrganisation = new GetOrganisation();
                    MembershipUser user = Membership.GetUser(User.Identity.Name);
                    organisation = objGetOrganisation.byUser(user);
                    if (organisation == null)
                        throw new Exception("Could not find Organisation");
                    Id = organisation.Id;
                }

                if (Request.IsAjaxRequest())
                {
                    if (Id != 0)
                    {
                        swdb.ContextOptions.LazyLoadingEnabled = false;
                        organisation = (swdb.Organisation.Include("Roles").Where(x => x.Id == Id)).FirstOrDefault();
                        if (organisation == null)
                            throw new Exception("Could not find Organisation");
                        var role = (from r in swdb.Roles where r.RoleName == roleName select r).FirstOrDefault();
                        //organisation.Roles.Remove(role);
                        organisation.Roles.Remove(role);
                        swdb.SaveChanges();
                    }

                 response.Success = true;
                    response.Message = "Role removed successfully!";
                    response.CssClass = "green";

                    return Json(response);
                }

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                if (Request.IsAjaxRequest())
                {
                    response.Success = false;
                    response.Message = ex.Message;
                    response.CssClass = "red";

                    return Json(response);
                }

                ModelState.AddModelError("", ex.Message);
            }

            return RedirectToAction("Index");
        }
        #endregion

        #region Delete Roles Methods

      

        #endregion

        #region Get Users In Role methods
     
        public ActionResult GetAllRoles([DefaultValue(0)] int Id)
        {
            try
            {
                var systemCode = ConfigurationManager.AppSettings["SystemCode"].ToString();
                var systemObj = swdb.Systems.FirstOrDefault(x => x.Code == systemCode);
                sw.Organisation organisation;
                string[] list;
                if (System.Web.Security.Roles.IsUserInRole(Settings.Default.OrganisationAdminRole) && !System.Web.Security.Roles.IsUserInRole("ADMINISTRATOR"))
                {
                    GetOrganisation objGetOrganisation = new GetOrganisation();
                    MembershipUser user = Membership.GetUser(User.Identity.Name);
                    organisation = objGetOrganisation.byUser(user);
                    if (organisation == null)
                        throw new Exception("Could not find Organisation");
                        Id = organisation.Id;
                }
                if (Id != 0)
                {
                    organisation = (swdb.Organisation.Where(x => x.Id == Id)).FirstOrDefault();
                    if (organisation == null)
                        throw new Exception("Could not find Organisation");
                    list = (organisation.Roles.Select(x => x.RoleName)).ToArray();
                }
                else {
                    list = systemObj.Roles.Select(x => x.RoleName).ToArray();//roleService.GetAllRoles();
                }

                List<SelectObject> selectList = new List<SelectObject>();

                foreach (var item in list)
                {
                    selectList.Add(new SelectObject() { caption = item, value = item });
                }

                return Json(selectList, JsonRequestBehavior.AllowGet);
            }catch(Exception ex){
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                return null;
            }
        }

        [HttpGet]
        public ActionResult GetUsersInRole(string roleName, [DefaultValue(0)] int Id)
        {
            try
            {
                sw.Organisation organisation;
                string[] list;

                if (System.Web.Security.Roles.IsUserInRole(Settings.Default.OrganisationAdminRole) && !System.Web.Security.Roles.IsUserInRole("ADMINISTRATOR"))
                {
                    GetOrganisation objGetOrganisation = new GetOrganisation();
                    MembershipUser user = Membership.GetUser(User.Identity.Name);
                    organisation = objGetOrganisation.byUser(user);
                    if (organisation == null)
                        throw new Exception("Could not find Organisation");
                    Id = organisation.Id;
                }
                if (Id != 0)
                {
                    swdb.ContextOptions.LazyLoadingEnabled = false;
                    organisation = (swdb.Organisation.Include("Users").Where(x => x.Id == Id)).FirstOrDefault();
                    if (organisation == null)
                        throw new Exception("Could not find Organisation");
                    list = (organisation.Users.Select(x=>x.UserName)).ToArray();
                }
                else {
                    list = roleService.GetUsersInRole(roleName);
                }
                return Json(list, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex) {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                return null;
            }
        }


        #endregion
    }
}
