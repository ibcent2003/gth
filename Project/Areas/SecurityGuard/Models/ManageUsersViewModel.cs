using System.Web.Security;
using Project.Models;
using SecurityGuard.Core.Pagination;
using sw = GNSW.DAL;

namespace SecurityGuard.ViewModels
{
    public class ManageUsersViewModel : PageInfoModel
    {
        public MembershipUserCollection Users { get; set; }
        public PaginatedList<MembershipUser> PaginatedUserList { get; set; }
        public string FilterBy { get; set; }
        public string SearchTerm { get; set; }
     //   public int PageSize { get; set; }

        public sw.Organisation objOrganisation { get; set; }

        public System.Collections.Generic.List<sw.Users> Rows { get; set; }

    }
}
