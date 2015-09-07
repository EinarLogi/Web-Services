using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace API.Models
{
    /// <summary>
    /// This class represents a single course, and contains various
    /// details about the course
    /// </summary>
    public class CourseDetailsDTO
    {
        /// <summary>
        /// Unique identifier for the course
        /// </summary>
        public int ID { get; set; }
        /// <summary>
        /// Name of the course
        /// Example: "Vefþjónustur"
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// List of students in the course
        /// </summary>
        public List<StudentDTO> Students { get; set; }

        /// <summary>
        /// Indicates how many students can be enrolled in a given course at any given time.
        /// Example: 35
        /// </summary>
        public int MaxStudents { get; set; }
    }
}
