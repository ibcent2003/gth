using System.Web.Security;
using sw = GNSW.DAL;

namespace SecurityGuard.ViewModels
{
    public class UserViewModel
    {
        public MembershipUser User { get; set; }
        public bool RequiresSecretQuestionAndAnswer { get; set; }
        public string[] Roles { get; set; }

        public sw.Organisation objOrganisation { get; set; }
    }
}
