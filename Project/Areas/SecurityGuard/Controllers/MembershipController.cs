using System;
using System.Linq;
using System.Web.Mvc;
using System.Web.Security;
using SecurityGuard.Core.Extensions;
using SecurityGuard.Services;
using SecurityGuard.Core.Attributes;
using routeHelpers = SecurityGuard.Core.RouteHelpers;
using SecurityGuard.Interfaces;
using SecurityGuard.ViewModels;
using Project.Controllers;
using viewModels = Project.Areas.SecurityGuard.ViewModels;
using Project.DAL;
using System.ComponentModel;
using System.Collections.Generic;
using Project.Properties;
using Project.UI.Models;
//using PAAR.BL;
using sw = GNSW.DAL;
using System.Configuration;
using Project.Models;
using Project.Areas.SecurityGuard.Models;

namespace Project.Areas.SecurityGuard.Controllers
{
  //  [Authorize(Roles = "ADMINISTRATOR, Organisation Admin")]
    public partial class MembershipController : BaseController
    {

        #region ctors
        private PROEntities db = new PROEntities();
        private sw.GNSWEntities swdb = new sw.GNSWEntities();
      //  private Crud crud = new Crud();
        private ProcessUtility utility = new ProcessUtility();
        private IMembershipService membershipService;
        private readonly IRoleService roleService;
        protected IEnumerable<sw.Users> rows;

        public MembershipController()
        {
            // this.db = this.crud.db;
            //this.appServices = new ApprovalServiceReference.ApprovalClient();
            this.roleService = new RoleService(System.Web.Security.Roles.Provider);
            this.membershipService = new MembershipService(Membership.Provider);
            this.rows = new List<sw.Users>();
        }

        #endregion

        #region Index Method
        //[Authorize(Roles = "Administrator, Organisation Admin")]
        public virtual ActionResult Index(string filterby, string searchterm, [DefaultValue(1)] int page, [DefaultValue(12)] int pgsize, [DefaultValue(0)] int Id)
        {
            try
            {
                sw.Organisation organisation = new sw.Organisation();
                ManageUsersViewModel viewModel = new ManageUsersViewModel();
                var systemCode = ConfigurationManager.AppSettings["SystemCode"].ToString();
                var systemObj = swdb.Systems.FirstOrDefault(x => x.Code == systemCode);
                var systemRoles = systemObj.Roles;//.Select(x=>x.RoleName).ToArray();
                IList<sw.Users> lUsers = new List<sw.Users>();
                viewModel.Users = null;
                viewModel.FilterBy = filterby;
                viewModel.SearchTerm = searchterm;

                var users = systemRoles.Select(x => x.Users).ToList();
                foreach (var uo in users)
                {
                    foreach (var u in uo)
                    {
                        lUsers.Add(u);
                    }
                }

                if (!string.IsNullOrEmpty(filterby))
                {
                    if (filterby == "all")
                    {

                        // rows = swdb.Roles.Where(x => systemRoles.Contains(x.RoleName)).Select(x=>x.Users);
                        rows = lUsers;

                    }
                    else if (!string.IsNullOrEmpty(searchterm))
                    {
                        string query = searchterm.Trim().ToUpper();
                        if (filterby == "email")
                        {
                            rows = lUsers.Where(u => u.Memberships.Email.ToUpper().Contains(query));
                        }
                        else if (filterby == "username")
                        {
                            rows = lUsers.Where(u =>u.UserName.ToUpper().Contains(query));
                        }
                    }
                    else
                    {
                        rows = lUsers;
                    }

                    if (System.Web.Security.Roles.IsUserInRole(Settings.Default.OrganisationAdminRole) && !System.Web.Security.Roles.IsUserInRole("Administrator"))
                    {
                        GetOrganisation objGetOrganisation = new GetOrganisation();
                        MembershipUser user = Membership.GetUser(User.Identity.Name);
                        organisation = objGetOrganisation.byUser(user);
                        if (organisation == null)
                            throw new Exception("Could not find Organisation");
                        Id = organisation.Id;

                        //viewModel.OrganizationId = organization.Id;
                        //Id = organization.Id;
                        //TempData["messageType"] = "alert-warning";
                        //TempData["message"] = "You are not Authorize to access all Users Account in the System";
                        //return RedirectToAction("IndexOrgAdmin", "Organization", new { area = "Setup" });
                    }

                    if (Id != 0)
                    {
                        organisation = (from o in swdb.Organisation where o.Id == Id select o).FirstOrDefault();
                        if (organisation == null)
                            throw new Exception("Could not find Organisation");
                        //viewModel.OrganizationId = OrganizationId;
                        if (!string.IsNullOrEmpty(filterby))
                        {
                            if (filterby == "all")
                            {
                                rows = organisation.Users;

                            }
                            else if (!string.IsNullOrEmpty(searchterm))
                            {
                                string query = searchterm.Trim().ToUpper();
                                if (filterby == "email")
                                {

                                    rows = organisation.Users.Where(x => x.Memberships.Email.ToUpper().Contains(query));
                                }
                                else if (filterby == "username")
                                {
                                    rows = organisation.Users.Where(x => x.UserName.ToUpper().Contains(query));
                                }
                            }
                            else
                            {
                                rows = organisation.Users;
                            }
                        }
                    }
                    //foreach (var username in usernames)
                    //{
                    //    rows.Add(membershipService.GetUser(username, false));
                    //}
                    viewModel.Rows = rows.Skip((page - 1) * pgsize).Take(pgsize).ToList();
                }
                viewModel.PagingInfo = new PagingInfo
                {
                    FirstItem = ((page - 1) * pgsize) + 1,
                    LastItem = page * pgsize,
                    CurrentPage = page,
                    ItemsPerPage = pgsize,
                    TotalItems = rows.Count()
                };
                viewModel.PageSize = pgsize;
                viewModel.objOrganisation = organisation;
                return View(viewModel);

            }
            catch (Exception ex)
            {
                //ToDo: Log with Elmah
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                TempData["message"] = Settings.Default.GenericExceptionMessage;
                return RedirectToAction("Index", "Home", new { area = "Admin" });
            }
        }
        #endregion

        #region Create User Methods
     //   [Authorize(Roles = "ADMINISTRATOR, Organisation Admin")]
        public virtual ActionResult CreateUser([DefaultValue(0)] int Id)
        {
            try
            {
                sw.Organisation organisation = new sw.Organisation();
                var model = new viewModels.RegisterViewModel();
                model.userDetails = new sw.UserDetails();
                model.ncsUserDetails = new sw.NCSUserDetails();

                model.RequireSecretQuestionAndAnswer = membershipService.RequiresQuestionAndAnswer;
                if (System.Web.Security.Roles.IsUserInRole(Settings.Default.OrganisationAdminRole) && !System.Web.Security.Roles.IsUserInRole("Administrator"))
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
                    organisation = (from o in swdb.Organisation where o.Id == Id select o).FirstOrDefault();
                    if (organisation == null)
                        throw new Exception("Could not find Organisation");

                    //model.OrganizationId = OrganizationId.ToString();
                }
                model.OfficerRank = new SelectList(swdb.OfficerRank, "Name", "Description");
                model.CustomsOfficeList = new SelectList(swdb.CustomsOffice, "ID", "Office");
                model.NCSOrganisationId = Settings.Default.NCSOrganisationId;
                model.objOrganisation = organisation;
                return View(model);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                TempData["message"] = Settings.Default.GenericExceptionMessage;
                return RedirectToAction("Index", "Home", new { area = "Admin" });
            }
        }

        /// <summary>
        /// This method redirects to the GrantRolesToUser method.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
     //  [Authorize(Roles = "ADMINISTRATOR, Organisation Admin")]
        [HttpPost]
        public virtual ActionResult CreateUser(viewModels.RegisterViewModel model)
        {
            sw.Organisation organisation = new sw.Organisation();
            model.objOrganisation = organisation;
            if (System.Web.Security.Roles.IsUserInRole(Settings.Default.OrganisationAdminRole) && !System.Web.Security.Roles.IsUserInRole("Administrator"))
            {
                GetOrganisation objGetOrganisation = new GetOrganisation();
                MembershipUser user = Membership.GetUser(User.Identity.Name);
                organisation = objGetOrganisation.byUser(user);
                if (organisation == null)
                    throw new Exception("Could not find Organisation");
                model.Id = organisation.Id;
                model.objOrganisation = organisation;
            }
            try
            {
                if (ModelState.IsValid)
                {
                    //var password = Membership.GeneratePassword(8, 0);
                    //model.Password = password;
                    //model.ConfirmPassword = password;
                    string message = "Welcome to Ghana's Trading Hub <br><br>Find below your account details<br><br>Username: " + model.UserName + " <br>Password: " + model.Password;
                  //  var subject = "New Account Details";
                    MembershipUser newUser;
                    MembershipCreateStatus status;
                    newUser = membershipService.CreateUser(model.UserName, model.Password, model.Email, model.SecretQuestion, model.SecretAnswer, model.Approve, out status);

                    if (status != MembershipCreateStatus.Success)
                    {
                        var msg = GetErrorMessage(status);
                        TempData["message"] = "alert-danger";
                        TempData["message"] = msg;
                        return RedirectToAction("Index", new { Id = model.Id });
                    }
                    else
                    {
                        try
                        {
                            if (model.Id != 0)
                            {
                                organisation = (from o in swdb.Organisation where o.Id == model.Id select o).FirstOrDefault();
                                if (organisation == null)
                                    throw new Exception("Could not find Organisation");
                                model.objOrganisation = organisation;
                                sw.Users user = swdb.Users.Where(x => x.UserId == (Guid)newUser.ProviderUserKey).FirstOrDefault();
                                organisation.Users.Add(user);
                                swdb.SaveChanges();

                                sw.UserDetails userDetails = new sw.UserDetails();
                                userDetails = model.userDetails;
                                userDetails.UserId = user.UserId;
                                userDetails.ModifiedBy = User.Identity.Name;
                                userDetails.ModifiedDate = DateTime.Now;

                                var record = (from x in swdb.UserDetails where x.UserId == userDetails.UserId select x).ToList();

                                if (record.Count > 0)
                                {
                                    swdb.UserDetails.Detach(record.FirstOrDefault());
                                    swdb.UserDetails.Attach(userDetails);
                                    swdb.ObjectStateManager.ChangeObjectState(userDetails, System.Data.EntityState.Modified);
                                    swdb.SaveChanges();
                                }
                                else
                                {
                                    swdb.UserDetails.AddObject(userDetails);
                                    swdb.SaveChanges();
                                }

                                sw.NCSUserDetails ncsUserDetails = new sw.NCSUserDetails();
                                ncsUserDetails = model.ncsUserDetails;
                                if (organisation.Id == Settings.Default.NCSOrganisationId)
                                {
                                    ncsUserDetails.UserId = user.UserId;
                                    ncsUserDetails.ModifiedBy = User.Identity.Name;
                                    ncsUserDetails.ModifiedDate = DateTime.Now;

                                    ////find NCS Details if exist
                                    var ncsrecord = (from x in swdb.NCSUserDetails where x.UserId == ncsUserDetails.UserId select x).ToList();
                                    if (ncsrecord.Count > 0)
                                    {
                                        swdb.NCSUserDetails.Detach(ncsrecord.FirstOrDefault());
                                        swdb.NCSUserDetails.Attach(ncsUserDetails);
                                        swdb.ObjectStateManager.ChangeObjectState(ncsUserDetails, System.Data.EntityState.Modified);
                                        swdb.SaveChanges();
                                    }
                                    else
                                    {
                                        swdb.NCSUserDetails.AddObject(ncsUserDetails);
                                        swdb.SaveChanges();
                                    }
                                }

                                ///Send Email to New User Account////
                                // @utility.SendEmailToPerson(Settings.Default.EmailReplyTo, newUser.Email, subject, message, DateTime.Now, null, null, null);
                                return RedirectToAction("GrantRolesToUser", new { username = newUser.UserName, Id = model.Id });
                            }
                            ///Send Email to New User Account////
                            // @utility.SendEmailToPerson(Settings.Default.EmailReplyTo, newUser.Email, subject, message, DateTime.Now, null, null, null);
                            return routeHelpers.Actions.GrantRolesToUser(newUser.UserName);
                            //RedirectToAction("GrantRolesToUser",new{UserName = '', Id = ''})
                        }
                        catch (Exception ex)
                        {
                            Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                            TempData["message"] = Settings.Default.GenericExceptionMessage;
                            if (status != MembershipCreateStatus.Success)
                                Membership.DeleteUser(newUser.UserName);
                        }
                    }
                }
            }
            catch (Exception ex)
            {


                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                TempData["message"] = Settings.Default.GenericExceptionMessage;
                //return RedirectToAction("Index", "Home", new { area = "Admin" });
            }
            model.OfficerRank = new SelectList(swdb.OfficerRank, "Name", "Description");
            model.CustomsOfficeList = new SelectList(swdb.CustomsOffice, "ID", "Office");
            model.NCSOrganisationId = Settings.Default.NCSOrganisationId;
            model.objOrganisation = organisation;
            return View(model);
        }

        public string GetErrorMessage(MembershipCreateStatus status)
        {
            switch (status)
            {
                case MembershipCreateStatus.DuplicateUserName:
                    return "Username already exists. Please enter a different user name.";

                case MembershipCreateStatus.DuplicateEmail:
                    return "A username for that e-mail address already exists. Please enter a different e-mail address.";

                case MembershipCreateStatus.InvalidPassword:
                    return "The password provided is invalid. Please enter a valid password value.";

                case MembershipCreateStatus.InvalidEmail:
                    return "The e-mail address provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidAnswer:
                    return "The password retrieval answer provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidQuestion:
                    return "The password retrieval question provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidUserName:
                    return "The user name provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.ProviderError:
                    return "The authentication provider returned an error. Please verify your entry and try again. If the problem persists, please contact your system administrator.";

                case MembershipCreateStatus.UserRejected:
                    return "The user creation request has been canceled. Please verify your entry and try again. If the problem persists, please contact your system administrator.";

                default:
                    return "An unknown error occurred. Please verify your entry and try again. If the problem persists, please contact your system administrator.";
            }
        }

        /// <summary>
        /// An Ajax method to check if a username is unique.
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult CheckForUniqueUser(string userName)
        {
            MembershipUser user = membershipService.GetUser(userName);
            JsonResponse response = new JsonResponse();
            response.Exists = (user == null) ? false : true;

            return Json(response, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region Delete User Methods
        [Authorize(Roles = "ADMINISTRATOR")]
        [HttpPost]
        [MultiButtonFormSubmit(ActionName = "UpdateDeleteCancel", SubmitButton = "DeleteUser")]
        public ActionResult DeleteUser(string UserName)
        {
            if (string.IsNullOrEmpty(UserName))
            {
                throw new ArgumentNullException("userName");
            }

            try
            {
                membershipService.DeleteUser(UserName);
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "There was an error deleting this user. - " + ex.Message;
            }

            return RedirectToAction("Update", new { userName = UserName });
        }



        #endregion

        #region View User Details Methods

        [HttpGet]
        public ActionResult Update(string userName, [DefaultValue(0)] int Id)
        {
            try
            {
                sw.Organisation organisation = new sw.Organisation();
                if (System.Web.Security.Roles.IsUserInRole(Settings.Default.OrganisationAdminRole) && !System.Web.Security.Roles.IsUserInRole("Administrator"))
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
                    organisation = (from o in swdb.Organisation where o.Id == Id select o).FirstOrDefault();
                    if (organisation == null)
                        throw new Exception("Could not find Organisation");
                }
                MembershipUser updateuser = membershipService.GetUser(userName);

                UserViewModel viewModel = new UserViewModel();
                viewModel.User = updateuser;
                viewModel.RequiresSecretQuestionAndAnswer = membershipService.RequiresQuestionAndAnswer;
                viewModel.Roles = roleService.GetRolesForUser(userName);
                viewModel.objOrganisation = organisation;

                return View(viewModel);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                TempData["message"] = Settings.Default.GenericExceptionMessage;
                return RedirectToAction("Index", "Home", new { area = "Admin" });
            }
        }

        [HttpPost]
        //[ActionName("Update")]
        [MultiButtonFormSubmit(ActionName = "UpdateDeleteCancel", SubmitButton = "UpdateUser")]
        public ActionResult UpdateUser(string UserName, [DefaultValue(0)] int Id)
        {
            if (string.IsNullOrEmpty(UserName))
            {
                throw new ArgumentNullException("userName");
            }

            MembershipUser user = membershipService.GetUser(UserName);

            try
            {
                user.Comment = Request["User.Comment"];
                user.Email = Request["User.Email"];

                membershipService.UpdateUser(user);
                TempData["SuccessMessage"] = "The user was updated successfully!";

            }
            catch (Exception)
            {
                TempData["ErrorMessage"] = "There was an error updating this user.";
            }

            return RedirectToAction("Update", new { userName = user.UserName, Id = Id });
        }


        #region Ajax methods for Updating the user

        [HttpPost]
        public ActionResult Unlock(string userName)
        {
            JsonResponse response = new JsonResponse();

            MembershipUser user = membershipService.GetUser(userName);

            try
            {
                user.UnlockUser();
                response.Success = true;
                response.Message = "User unlocked successfully!";
                response.Locked = false;
                response.LockedStatus = (response.Locked) ? "Locked" : "Unlocked";
            }
            catch (Exception)
            {
                response.Success = false;
                response.Message = "User unlocked failed.";
            }

            return Json(response);
        }


        [HttpPost]
        public ActionResult ApproveDeny(string userName)
        {
            JsonResponse response = new JsonResponse();

            MembershipUser user = membershipService.GetUser(userName);

            try
            {
                user.IsApproved = !user.IsApproved;
                membershipService.UpdateUser(user);

                string approvedMsg = (user.IsApproved) ? "Approved" : "Denied";

                response.Success = true;
                response.Message = "User " + approvedMsg + " successfully!";
                response.Approved = user.IsApproved;
                response.ApprovedStatus = (user.IsApproved) ? "Approved" : "Not approved";
            }
            catch (Exception)
            {
                response.Success = false;
                response.Message = "User unlocked failed.";
            }

            return Json(response);
        }

        #endregion

        #endregion

        #region Cancel User Methods

        [HttpPost]
        [MultiButtonFormSubmit(ActionName = "UpdateDeleteCancel", SubmitButton = "UserCancel")]
        public ActionResult Cancel()
        {
            return RedirectToAction("Index");
        }

        #endregion



        #region Grant Users with Roles Methods

        /// <summary>
        /// Return two lists:
        ///   1)  a list of Roles not granted to the user
        ///   2)  a list of Roles granted to the user
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
      // [Authorize(Roles = "ADMINISTRATOR, Organisation Admin")]
        public virtual ActionResult GrantRolesToUser(string username, [DefaultValue(0)] int Id)
        {
            try
            {
                sw.Organisation organisation = new sw.Organisation();
                var systemCode = ConfigurationManager.AppSettings["SystemCode"].ToString();
                var systemObj = swdb.Systems.FirstOrDefault(x => x.Code == systemCode);
                if (System.Web.Security.Roles.IsUserInRole(Settings.Default.OrganisationAdminRole) && !System.Web.Security.Roles.IsUserInRole("Administrator"))
                {
                    GetOrganisation objGetOrganisation = new GetOrganisation();
                    MembershipUser user = Membership.GetUser(User.Identity.Name);
                    organisation = objGetOrganisation.byUser(user);
                    if (organisation == null)
                        throw new Exception("Could not find Organisation");
                    Id = organisation.Id;
                }
                if (string.IsNullOrEmpty(username))
                {
                    return RedirectToAction("Index", new { Id = Id });
                }
                GrantRolesToUserViewModel model = new GrantRolesToUserViewModel();
                model.UserName = username;
                model.AvailableRoles = (string.IsNullOrEmpty(username) ? new SelectList(systemObj.Roles.Select(x => x.RoleName)) : new SelectList(systemObj.Roles.Select(x => x.RoleName)));
                model.GrantedRoles = (string.IsNullOrEmpty(username) ? new SelectList(new string[] { }) : new SelectList(roleService.GetRolesForUser(username)));

                if (Id != 0)
                {
                    organisation = (from o in swdb.Organisation where o.Id == Id select o).FirstOrDefault();

                    var roles = organisation.Roles.Select(x => x.RoleName).ToList();
                    model.AvailableRoles = (string.IsNullOrEmpty(username) ? new SelectList(new string[] { }) : new SelectList(roles.Except(roleService.GetRolesForUser(username))));
                }
                model.objOrganisation = organisation;
                //model.Id = Id;
                return View(model);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                TempData["message"] = Settings.Default.GenericExceptionMessage;
                return RedirectToAction("Index", "Home", new { area = "Admin" });
            }
        }

        /// <summary>
        /// Grant the selected roles to the user.
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="roleNames"></param>
        /// <returns></returns>
    //  [Authorize(Roles = "Administrator, Organisation Admin")]
        [HttpPost]
        public virtual ActionResult GrantRolesToUser(string userName, string roles)
        {
            JsonResponse response = new JsonResponse();

            if (string.IsNullOrEmpty(userName))
            {
                response.Success = false;
                response.Message = "The userName is missing.";
                return Json(response);
            }

            string[] roleNames = roles.Substring(0, roles.Length - 1).Split(',');

            if (roleNames.Length == 0)
            {
                response.Success = false;
                response.Message = "No roles have been granted to the user.";
                return Json(response);
            }

            try
            {
                foreach (var role in roleNames)
                {
                    if (!roleService.IsUserInRole(userName, role))
                    {
                        roleService.AddUserToRole(userName, role);
                    }
                }

                response.Success = true;
                response.Message = "The Role(s) has been GRANTED successfully to " + userName;
            }
            catch (Exception)
            {
                response.Success = false;
                response.Message = "There was a problem adding the user to the roles.";
            }

            return Json(response);
        }

        /// <summary>
        /// Revoke the selected roles for the user.
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="roleNames"></param>
        /// <returns></returns>
    //    [Authorize(Roles = "Administrator, Organisation Admin")]
        [HttpPost]
        public ActionResult RevokeRolesForUser(string userName, string roles)
        {
            JsonResponse response = new JsonResponse();

            if (string.IsNullOrEmpty(userName))
            {
                response.Success = false;
                response.Message = "The userName is missing.";
                return Json(response);
            }

            if (string.IsNullOrEmpty(roles))
            {
                response.Success = false;
                response.Message = "Roles is missing";
                return Json(response);
            }

            string[] roleNames = roles.Substring(0, roles.Length - 1).Split(',');

            if (roleNames.Length == 0)
            {
                response.Success = false;
                response.Message = "No roles are selected to be revoked.";
                return Json(response);
            }

            try
            {

                foreach (var role in roleNames)
                {
                    if (roleService.IsUserInRole(userName, role))
                    {
                        roleService.RemoveUserFromRole(userName, role);
                    }
                }
                response.Success = true;
                response.Message = "The Role(s) has been REVOKED successfully for " + userName;
            }
            catch (Exception)
            {
                response.Success = false;
                response.Message = "There was a problem revoking roles for the user.";
            }

            return Json(response);
        }

        #endregion


        #region Grant users to roles latest

        public ActionResult GrantRole(string Id)
        {
            try
            {
                GrantRoleViewModel model = new GrantRoleViewModel();
                model.AllUsers = (from a in swdb.Users orderby a.UserName select a).AsEnumerable().Select(x => new SelectListItem() { Value = x.UserId.ToString(), Text = x.UserName }).ToList();
                if (Id == null || Id == "")
                {
                    TempData["messageType"] = "danger";
                    TempData["message"] = Settings.Default.GenericExceptionMessage;
                    return RedirectToAction("Index", "Dashboard", new { area = "Admin" });
                }
                else
                {
                    model.Id = Id;
                    var me = Guid.Parse(Id);
                    var GetUser = swdb.Users.Where(x => x.UserId == me).FirstOrDefault();
                    var GetUserRole = GetUser.Roles.Select(x => x.RoleId).ToList();
                    var GetAllRole = (from r in swdb.Roles select r.RoleId).ToList().Except(GetUserRole);

                    model.AllGrantedRole = GetUser.Roles.OrderBy(x => x.RoleName).ToList();

                    model.AllRole = (from r in swdb.Roles
                                     orderby r.RoleName
                                     where GetAllRole.Contains(r.RoleId)
                                     select r).ToList();
                }
                return View(model);
            }
            catch (Exception ex)
            {
                TempData["messageType"] = "danger";
                TempData["message"] = "An Error occur. Please try again later or contact the system administrator";
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        public ActionResult Grant(GrantRoleViewModel model)
        {
            try
            {
                var me = Guid.Parse(model.Id);
                var GetUser = swdb.Users.Where(x => x.UserId == me).FirstOrDefault();
                var GetUserRole = GetUser.Roles.Select(x => x.RoleId).ToList();
                var GetAllRole = (from r in swdb.Roles select r.RoleId).ToList().Except(GetUserRole);

                model.AllRole = (from r in swdb.Roles
                                 orderby r.RoleName
                                 select r).ToList();
                if (model.RoleUsed == null)
                {
                    TempData["messageType"] = "danger";
                    TempData["message"] = "Please select at least ONE Role to grant";
                    return RedirectToAction("GrantRole", "Membership", new { area = "SecurityGuard", Id = model.Id });
                }
                string roletext = "";
                model.RoleUsedTex = new List<string> { };
                foreach (string roleId in model.RoleUsed)
                {
                    var RId = Guid.Parse(roleId.ToString());
                    roletext = (from p in swdb.Roles where p.RoleId == RId select p.RoleName).FirstOrDefault();
                    model.RoleUsedTex.Add(roletext);
                }
                foreach (string p in model.RoleUsed.ToList())
                {
                    var RsId = Guid.Parse(p.ToString());
                    var RoleN = (from r in swdb.Roles where r.RoleId == RsId select r).FirstOrDefault();
                    if (model.RoleUsed != null)
                    {
                        GetUser.Roles.Add(RoleN);
                        swdb.SaveChanges();
                    }
                }
                TempData["message"] = "The Role(s) has been GRANTED successfully to " + GetUser.UserName.ToUpper() + "";
                return RedirectToAction("GrantRole", "Membership", new { area = "SecurityGuard", Id = model.Id });
            }
            catch (Exception ex)
            {
                TempData["messageType"] = "danger";
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                TempData["message"] = Settings.Default.GenericExceptionMessage;
                return RedirectToAction("Index", "Dashboard", new { area = "Admin" });
            }
        }



        [HttpPost]
        public ActionResult Revoke(GrantRoleViewModel model)
        {
            try
            {
                var me = Guid.Parse(model.Id);
                var GetUser = swdb.Users.Where(x => x.UserId == me).FirstOrDefault();
                var GetUserRole = GetUser.Roles.Select(x => x.RoleId).ToList();
                var GetAllRole = (from r in swdb.Roles select r.RoleId).ToList().Except(GetUserRole);
                model.AllGranted = GetUser.Roles.ToList();
                if (model.GrantedUsed == null)
                {
                    TempData["messageType"] = "danger";
                    TempData["message"] = "Please select at least ONE Role to revoke";
                    return RedirectToAction("GrantRole", "Membership", new { area = "SecurityGuard", Id = model.Id });
                }

                string roletext = "";
                model.RoleUsedTex = new List<string> { };

                foreach (string roleId in model.GrantedUsed)
                {
                    var RId = Guid.Parse(roleId.ToString());
                    roletext = (from p in swdb.Roles where p.RoleId == RId select p.RoleName).FirstOrDefault();
                    model.RoleUsedTex.Add(roletext);

                    var existingRole = GetUser.Roles.Where(x => x.RoleId == RId).ToList();
                    if (existingRole != null)
                    {
                        foreach (var role in existingRole)
                        {
                            var RsId = Guid.Parse(role.RoleId.ToString());
                            var RoleN = (from r in swdb.Roles where r.RoleId == RsId select r).FirstOrDefault();
                            if (RoleN != null)
                            {
                                GetUser.Roles.Remove(RoleN);
                                swdb.SaveChanges();
                            }
                        }
                    }
                }
                TempData["message"] = "The Role(s) has been REVOKED successfully to " + GetUser.UserName.ToUpper() + "";
                return RedirectToAction("GrantRole", "Membership", new { area = "SecurityGuard", Id = model.Id });
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                TempData["messageType"] = "danger";
                TempData["message"] = Settings.Default.GenericExceptionMessage;
                return RedirectToAction("Index", "Dashboard", new { area = "Admin" });
            }
        }
        #endregion

    }
}
