using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Project.Areas.Admin.Models
{
    public class DashboardViewModel
    {

        public int NoOfGeneralGoods
        {
            get;
            set;
        }

        public int NoOfUsedDuty
        {
            get;
            set;
        }

        public int TotalConsigmentUsed
        {
            get;
            set;
        }

        public int TotalDocument
        {
            get;
            set;
        }

        public int TotalDutyFeedback
        {
            get;
            set;
        }

        public int TotalNews
        {
            get;
            set;
        }

        public int TotalOrg
        {
            get;
            set;
        }

        public int TotalUserFeedback
        {
            get;
            set;
        }

        public int TotalUsersVisit
        {
            get;
            set;
        }

    }
}