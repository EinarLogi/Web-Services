using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace API.Models.Courses
{
    /// <summary>
    /// This class represents the data needed to update a course.
    /// </summary>
    public class CourseUpdateViewModel
    {
        /// <summary>
        /// 
        /// </summary>
        public DateTime StartDate { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public DateTime EndDate { get; set; }
        /// <summary>
        /// 
        /// 
        /// </summary>
        public string Semester { get; set; }

    }
}
