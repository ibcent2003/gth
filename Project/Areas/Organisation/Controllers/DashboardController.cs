using CHAMS.DAL;
using Elmah;
using sw=GNSW.DAL;
using Project.Areas.Organisation.Models;
using Project.Areas.Setup.Models;
using Project.DAL;
using Project.Models;
using Project.Properties;
using Project.UI.Models;
using System;
using System.Collections.Generic;
using System.Data.Objects;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Security.Principal;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace Project.Areas.Organisation.Controllers
{
    public class DashboardController : Controller
    {
        //
        // GET: /Organisation/Dashboard/

        private PROEntities db = new PROEntities();

        private ProcessUtility util;

        private sw.GNSWEntities swdb = new sw.GNSWEntities();

        private CHAMSEntities chamsdb = new CHAMSEntities();

        private string filePath;

        private string docPath;

        private HandlingService AppModel = new HandlingService();

        public DashboardController()
        {
            this.util = new ProcessUtility();
            this.filePath = "~/Content/Backend/News/";
            this.docPath = "~/Content/Frontend/light/img/sections/Documents/";
        }

        private sw.Organisation getOrganisationDetails()
        {
            try
            {
                var username = Membership.GetUser().UserName;
                var user = this.swdb.Users.Where(x => x.UserName == username).FirstOrDefault();
                return user.Organisation.FirstOrDefault();
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                return null;
            }
        }
        public ActionResult Index()
        {
           try
            {
                if (this.getOrganisationDetails() == null)
                {
                    ErrorSignal.FromCurrentContext().Raise(new Exception("The User doesnt belong to any organisation on GNSW"));
                    return RedirectToAction("Index", "Dashboard", new { area = "Admin" });
                }
                OrganisationDashboardModel model = new OrganisationDashboardModel();
                return View(model);
            }
            catch(Exception ex)
            {
                Exception exception = ex;
                base.TempData["messageType"] = "alert-danger";
                ErrorSignal.FromCurrentContext().Raise(exception);
                return RedirectToAction("Index", "Dashboard", new { area = "Admin" });
            }

        }

    }
}
