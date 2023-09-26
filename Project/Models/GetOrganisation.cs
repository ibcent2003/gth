using Project.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using sw = GNSW.DAL;

namespace Project.Models
{
    public class GetOrganisation
    {
        private PROEntities db = new PROEntities();
        private sw.GNSWEntities swdb = new sw.GNSWEntities();
        public GetOrganisation()
        {
        }

        public sw.Organisation byUser(MembershipUser user)
        {

            sw.Organisation organisation = (from u in swdb.Users
                                            where u.UserId == (Guid)user.ProviderUserKey
                                            select u.Organisation.FirstOrDefault()).FirstOrDefault();
            return organisation;
        }
    }
}