using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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
        /// Example: "15.08.2015"
        /// </summary>
        [Required]
        public DateTime StartDate { get; set; }

        /// <summary>
        /// Example: "20.11.2015"
        /// </summary>
        [Required]
        public DateTime EndDate { get; set; }

        /// <summary>
        /// Indicates how many students can be enrolled in a given course at any given time.
        /// Example: 35
        /// </summary>
        public int MaxStudents { get; set; }
    }
}
