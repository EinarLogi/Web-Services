using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using API.Models;

namespace API.Services
{
    public class CoursesServiceProvider
    {
        /// <summary>
        /// Returns a list of courses belonging to a given semester.
        /// If no semester is provided, the "current" semester will
        /// be used.
        /// </summary>
        /// <param name="semester"></param>
        /// <returns></returns>
        public List<CourseDTO> GetCoursesBySemester(string semester = null)
        {
            if (string.IsNullOrEmpty(semsester))
            {
                semester = "20153";
            }
            //TODO: get all courses belonging to "semester"
        }
    }
}
