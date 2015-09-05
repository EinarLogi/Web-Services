using API.Models;
using API.Models.Courses;
using API.Models.Courses.Students;
using API.Services;
using API.Services.Exception;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;

namespace Assignment2.Controllers
{
    /// <summary>
    /// This alert represents...
    /// </summary>
    [RoutePrefix("api/courses")]
    public class CoursesController : ApiController
    {
        private readonly CoursesServiceProvider _service;

        public CoursesController()
        {
            _service = new CoursesServiceProvider();
        }

        /// <summary>
        /// Returns a list of courses
        /// </summary>
        /// <returns>A list of courses</returns>
        [HttpGet]
        [Route("")]
        public List<CourseDTO> GetCourses(string semester = null)
        {
            return _service.GetCoursesBySemester(semester);
        }

        /// <summary>
        /// Update information of a course with a given id
        /// </summary>
        /// <param name="id"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("{id}")]
        public CourseDTO UpdateCourse(int id, CourseUpdateViewModel model)
        {
            try
            {
                return _service.UpdateCourse(id, model);
            }
            catch (AppObjectNotFoundException)
            {
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }
            
        }

        /// <summary>
        /// Returns detailed information about a course.
        /// </summary>
        /// <returns>A CourseDetailsDTO object</returns>
        [HttpGet]
        [Route("{id}")]
        [ResponseType(typeof(CourseDetailsDTO))]
        public IHttpActionResult GetCourseById(int id)
        {
            try
            {
                // ATH: Er rétt að returna OK?
                var result = _service.GetCourseById(id);
                return Content(HttpStatusCode.OK, result);
            }
            catch (AppObjectNotFoundException)
            {
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }
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

        /// <summary>
        /// Adds a student to a course with a given ID
        /// </summary>
        /// <param name="id"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("{id}/students")]
        [ResponseType(typeof(StudentDTO))]
        public IHttpActionResult AddStudentToCourse(int id, AddStudentViewModel model)
        {
            //TODO
            //1.Validation

            if (ModelState.IsValid)
            {
                try
                {
                    var result = _service.AddStudentToCourse(id, model);
                    return Content(HttpStatusCode.Created, result);
                }
                catch(AppObjectNotFoundException)
                {
                    return StatusCode(HttpStatusCode.NotFound);
                }
                
            }
            else
            {
                return StatusCode(HttpStatusCode.PreconditionFailed);
            }
        }


    }
}