using System.Collections.Generic;
using System.Web.Mvc;

namespace SecurityGuard.ViewModels
{
    public class ManageRolesViewModel
    {
        public SelectList Roles { get; set; }
        public string[] RoleList { get; set; }

        public GNSW.DAL.Organisation objOrganisation { get; set; }

        public SelectList AvailableRoles { get; set; }
    }
}
