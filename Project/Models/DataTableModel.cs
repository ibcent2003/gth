using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Project.Models
{
    public class DataTableModel
    {
        /// &lt;summary>
        /// Request sequence number sent by DataTable,
        /// same value must be returned in response
        /// &lt;/summary>       
        public string sEcho { get; set; }

        /// &lt;summary>
        /// Text used for filtering
        /// &lt;/summary>
        public string sSearch { get; set; }

        /// &lt;summary>
        /// Number of records that should be shown in table
        /// &lt;/summary>
        public int iDisplayLength { get; set; }

        /// &lt;summary>
        /// First record that should be shown(used for paging)
        /// &lt;/summary>
        public int iDisplayStart { get; set; }

        /// &lt;summary>
        /// Number of columns in table
        /// &lt;/summary>
        public int iColumns { get; set; }

        /// &lt;summary>
        /// Number of columns that are used in sorting
        /// &lt;/summary>
        public int iSortingCols { get; set; }

        /// &lt;summary>
        /// Comma separated list of column names
        /// &lt;/summary>
        public string sColumns { get; set; }
    }
}