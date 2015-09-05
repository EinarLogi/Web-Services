using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace API.Models
{
    /// <summary>
    /// This class represents the data necessary to create
    /// a single course taught in a given semester.
    /// </summary>
    public class CourseViewModel
    {
        /// <summary>
        /// The ID of the course being created.
        /// Example:"T-514-VEFT
        /// </summary>
        [Required]
        public string CourseID { get; set; }

        /// <summary>
        /// The semester in which the course is taught
        /// Example: "20153"
        /// </summary>
        [Required]
        public string  Semester { get; set; }
    }
}
