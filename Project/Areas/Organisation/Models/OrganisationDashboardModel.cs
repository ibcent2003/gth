using sw =GNSW.DAL;
using Project.Areas.Setup.Models;
using Project.DAL;
using Project.Models;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Project.Areas.Organisation.Models
{
    public class OrganisationDashboardModel
    {
        public IList<NewsCategory> CategoryNews
        {
            get;
            set;
        }

        public IList<DocumentCategory> docCategoryList
        {
            get;
            set;
        }

        public List<IntegerSelectListItem> DocumentCategoryList
        {
            get;
            set;
        }

        //public DocumentForm documentform
        //{
        //    get;
        //    set;
        //}

        public IList<sw.DocumentInfo> documentlist
        {
            get;
            set;
        }

        public News news
        {
            get;
            set;
        }

        public List<IntegerSelectListItem> NewsCategory
        {
            get;
            set;
        }

        public NewsForm newsform
        {
            get;
            set;
        }

        public sw.Organisation Organisation
        {
            get;
            set;
        }

        public string path
        {
            get;
            set;
        }

        public string PicturePath
        {
            get;
            set;
        }

        public IList<News> Rows
        {
            get;
            set;
        }

        public int TotalApprovedNews
        {
            get;
            set;
        }

        public int TotalDeletedNews
        {
            get;
            set;
        }

        public int TotalDocuments
        {
            get;
            set;
        }

        public int TotalNews
        {
            get;
            set;
        }

    }
}