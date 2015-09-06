using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace API.Models.Courses.Students
{
    /// <summary>
    /// This class represents the data needed to add a new student
    /// </summary>
    public class AddStudentViewModel
    {
        /// <summary>
        /// The SSN of the student
        /// </summary>
        [Required]
        public string SSN { get; set; }

    }
}
