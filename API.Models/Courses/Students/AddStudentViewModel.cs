using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace API.Models.Courses.Students
{
    public class AddStudentViewModel
    {
        [Required]
        public string SSN { get; set; }

    }
}
