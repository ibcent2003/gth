using Project.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Project.Models
{
    public class HelpViewModel
    {
        public List<FAQ> FAQList { get; set; }

        public List<CommonlyUsedTerms> UsedTermList { get; set; }

        public List<Guideline> GuidelineList { get; set; }

        public List<UsefulLink> UsefulLink { get; set; }

        public TermsAndConditions TermsCondition { get; set; }

        public TermsOfUsed GNSWTerms { get; set; }

        public string DownloadPath { get; set; }

        public string MasterUrl { get; set; }
    }
}