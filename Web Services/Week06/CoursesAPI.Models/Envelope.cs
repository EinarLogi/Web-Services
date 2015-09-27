using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoursesAPI.Models
{
    /// <summary>
    /// This generic class represents a envelope returned to the client
    /// when reqesting a page.
    /// The envelope contains the content of the page and info about the page.
    /// </summary>
    /// <typeparam name="T">The content of the page</typeparam>
    public class Envelope<T>
    {
        /// <summary>
        /// Stores the content of the page
        /// </summary>
        public T Items { get; set; }
        /// <summary>
        /// Stores the info about the page
        /// </summary>
        public PagingInfo Paging { get; set; }
    }
}

