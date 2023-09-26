using System.Collections.Generic;
using System.Web.Mvc;
using System.Web.Security;
using sw = GNSW.DAL;

namespace SecurityGuard.ViewModels
{
    public class GrantRolesToUserViewModel
    {
        public MembershipUser User { get; set; }
        public string UserName { get; set; }
        public SelectList AvailableRoles { get; set; }
        public SelectList GrantedRoles { get; set; }

        public sw.Organisation objOrganisation { get; set; }

        public int Id { get; set; }
    }
}
