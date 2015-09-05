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
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// List of students in the course
        /// </summary>
        public List<StudentDTO> Students { get; set; }
    }
}
