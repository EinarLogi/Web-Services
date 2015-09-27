using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoursesAPI.Models
{
    /// <summary>
    /// This class represents info about the page returned to the client.
    /// </summary>
    public class PagingInfo
    {
        /// <summary>
        /// Stores the number of pages
        /// </summary>
        public int PageCount { get; set; }
        /// <summary>
        /// Stores the number of items in each page
        /// </summary>
        public int PageSize { get; set; }
        /// <summary>
        /// A 1-based index of the current page being returned
        /// </summary>
        public int PageNumber { get; set; }
        /// <summary>
        /// Stores the total number of items in the collection
        /// </summary>
        public int TotalNumberOfItems { get; set; }
    }
}
