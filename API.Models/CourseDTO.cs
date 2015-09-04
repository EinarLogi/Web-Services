using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace API.Models
{
    /// <summary>
    /// This class represents a item in a list of courses
    /// </summary>
    public class CourseDTO
    {
        /// <summary>
        /// Database-generated unique identifier of the course.
        /// Example: 1087
        /// </summary>
        public int ID { get; set; }
        /// <summary>
        /// The name of the course
        /// Example: "Vefþjónustur"
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// The date when the course starts
        /// The time portion is unused
        /// Example: "20.08.2015"
        /// </summary>
        public DateTime StartDate { get; set; }

    }
}
