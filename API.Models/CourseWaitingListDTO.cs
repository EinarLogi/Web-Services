using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace API.Models
{
    /// <summary>
    /// A class representing a list of all active students waiting for
    /// availible slot in a course.
    /// </summary>
    public class CourseWaitingListDTO
    {
        /// <summary>
        /// A list containing students on a waiting list for a course.
        /// </summary>
        public List<StudentDTO> WaitingStudents { get; set; }
    }
}
