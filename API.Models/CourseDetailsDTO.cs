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
        public int ID { get; set; }
        public string Name { get; set; }
        public List<StudentDTO> Students { get; set; }
        public string Description { get; set; }

    }
}
