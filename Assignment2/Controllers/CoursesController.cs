using API.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Assignment2.Controllers
{
    /// <summary>
    /// This alert represents...
    /// </summary>
    [RoutePrefix("api/courses")]
    public class CoursesController : ApiController
    {
        private List<CourseDTO> _courses;

        public CoursesController()
        {
            _courses = new List<CourseDTO>();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("")]
        public List<CourseDTO> GetCourses()
        {
            return _courses;
        }

        /// <summary>
        /// Returns a list of active students in a course
        /// </summary>
        /// <returns>A list of student objects</returns>
        [HttpGet]
        [Route("{id}/students")]
        public List<StudentDTO> GetStudentsInCourse()
        {
            return null;
        }
    } 
}
