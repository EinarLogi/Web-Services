using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using API.Models;
using API.Services.Repositories;

namespace API.Services
{
    /// <summary>
    /// This class handles the business logic
    /// </summary>
    public class CoursesServiceProvider
    {
        private readonly AppDataContext _db;

        public CoursesServiceProvider()
        {
            _db = new AppDataContext();
        }

        /// <summary>
        /// Returns a list of courses belonging to a given semester.
        /// If no semester is provided, the "current" semester will
        /// be used.
        /// </summary>
        /// <param name="semester"></param>
        /// <returns></returns>
        public List<CourseDTO> GetCoursesBySemester(string semester = null)
        {
            if (string.IsNullOrEmpty(semester))
            {
                semester = "20153";
            }

            var result = (from c in _db.Courses
                          join ct in _db.CourseTemplates
                                  on c.CourseIdentifier equals ct.CourseID
                          where c.Semester == semester
                          select new CourseDTO
                          {
                            ID              = c.ID,
                            StartDate       = c.StartDate,
                            Name            = ct.Name,
                            StudentCount    = 0 // TODO!! Þarf ad baeta vid tolfu fyrir numendur o.flr.
                           }).ToList();

            return result;
        }
    }
}
