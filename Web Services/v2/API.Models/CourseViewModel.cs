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
        public string TemplateID { get; set; }

        /// <summary>
        /// The semester in which the course is taught
        /// Example: "20153"
        /// </summary>
        [Required]
        public string  Semester { get; set; }

        /// <summary>
        /// Represents the date of the first lecture in the semester
        /// Example: "20160818T00:00:00"
        /// </summary>
        [Required]
        public DateTime StartDate { get; set; }

        /// <summary>
        /// Represents the the date of the last lecture in the semester
        /// Example: "2016-11-10T00:00:00"
        /// </summary>
        [Required]
        public DateTime EndDate { get; set; }

        /// <summary>
        /// Represents the maximum amount of allowed students in a course
        /// </summary>
        [Required]
        public int MaxStudents { get; set; }
    }
}
