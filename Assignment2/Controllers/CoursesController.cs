using API.Models;
using API.Models.Courses;
using API.Models.Courses.Students;
using API.Services;
using API.Services.Exception;
using API.Services.Exceptions;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;

namespace Assignment2.Controllers
{
    /// <summary>
    /// The controller for the Course API
    /// </summary>
    [RoutePrefix("api/courses")]
    public class CoursesController : ApiController
    {
        private readonly CoursesServiceProvider _service;

        /// <summary>
        /// Constructor that initializes _service.
        /// </summary>
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
        [ResponseType(typeof(List<CourseDTO>))]
        public IHttpActionResult GetCourses(string semester = null)
        {
            var result = _service.GetCoursesBySemester(semester);
            return Content(HttpStatusCode.OK, result);
        }

        /// <summary>
        /// Adds a new course to the database
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("")]
        [ResponseType(typeof(CourseDTO))]
        public IHttpActionResult AddNewCourse(CourseViewModel model)
        {
            try
            {
                var result = _service.AddNewCourse(model);
                var location = Url.Link("GetCourseById", new { id = result.ID });
                return Created(location, result);
            }
            catch (DuplicateEntryException)
            {

                return StatusCode(HttpStatusCode.BadRequest);
            };
        }


        /// <summary>
        /// Update information of a course with a given id
        /// </summary>
        /// <param name="id">The id of the course to be updated</param>
        /// <param name="model">The data needed tu update a course</param>
        /// <returns>The updated course</returns>
        [HttpPut]
        [Route("{id}")]
        [ResponseType(typeof(CourseDTO))]
        public IHttpActionResult UpdateCourse(int id, CourseUpdateViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var result = _service.UpdateCourse(id, model);
                    return Content(HttpStatusCode.OK, result);
                }
                catch (AppObjectNotFoundException)
                {
                    return StatusCode(HttpStatusCode.NotFound);
                }
            }
            else
            {
                return StatusCode(HttpStatusCode.PreconditionFailed);
            }
        }

        /// <summary>
        /// Deletes a specific course from the database given by id.
        /// </summary>
        /// <param name="id">The id of the course to be removed</param>
        /// <returns>A statuscode</returns>
        [HttpDelete]
        [Route("{id}")]
        public IHttpActionResult DeleteCourseById(int id)
        {
            try
            {
                _service.DeleteCourseById(id);

                IHttpActionResult response;
                HttpResponseMessage responsemsg = new HttpResponseMessage(HttpStatusCode.NoContent);
                response = ResponseMessage(responsemsg);
                return response;
            }
            catch (AppObjectNotFoundException)
            {

                return StatusCode(HttpStatusCode.NotFound);
            }
        }

        /// <summary>
        /// Returns a detailed information about a course with  a
        /// given ID.
        /// </summary>
        /// <param name="id">The ID of the requested course</param>
        /// <returns>A CourseDetailsDTO object</returns>
        [HttpGet]
        [Route("{id}", Name ="GetCourseById")]
        [ResponseType(typeof(CourseDetailsDTO))]
        public IHttpActionResult GetCourseById(int id)
        {
            try
            {
                var result = _service.GetCourseById(id);
                return Content(HttpStatusCode.OK, result);
            }
            catch (AppObjectNotFoundException)
            {
                return StatusCode(HttpStatusCode.NotFound);
            }
        }

        /// <summary>
        /// Returns a list of active students in a course
        /// </summary>
        /// <returns>A list of student objects</returns>
        [HttpGet]
        [Route("{id}/students")]
        public IHttpActionResult GetStudentsInCourse(int id)
        {
            try
            {
                var result = _service.GetStudentsInCourse(id);
                return Content(HttpStatusCode.OK, result);
            }
            catch (AppObjectNotFoundException)
            {

                return StatusCode(HttpStatusCode.NotFound);
            }
          
        }

        /// <summary>
        /// Adds a student to a course with a given ID
        /// </summary>
        /// <param name="id">The ID of the course</param>
        /// <param name="model">A model containing the SSN of the student</param>
        /// <returns></returns>
        [HttpPost]
        [Route("{id}/students")]
        [ResponseType(typeof(StudentDTO))]
        public IHttpActionResult AddStudentToCourse(int id, AddStudentViewModel model)
        {
            // Validation
            if (ModelState.IsValid)
            {
                try
                {
                    // Add student to the course and return the StudentDTO
                    var result = _service.AddStudentToCourse(id, model);
                    return Content(HttpStatusCode.Created, result);
                }
                catch(AppObjectNotFoundException)
                {
                    return StatusCode(HttpStatusCode.NotFound);
                }
                catch (MaxStudentException)
                {
                    return StatusCode(HttpStatusCode.PreconditionFailed);
                }
                catch (DuplicateEntryException)
                {
                    return StatusCode(HttpStatusCode.PreconditionFailed);
                }
                
            }
            else
            {
                return StatusCode(HttpStatusCode.PreconditionFailed);
            }
        }

        /// <summary>
        /// Returns a list of active students on the waiting list
        /// </summary>
        /// <param name="id">ID of the course</param>
        /// <returns>Active students on the waiting list.</returns>
        [HttpGet]
        [Route("{id}/waitinglist")]
        [ResponseType(typeof(List<StudentDTO>))]
        public IHttpActionResult GetCourseWaitingList(int id)
        {
            try
            {
                var result = _service.GetCourseWaitingList(id);
                return Content(HttpStatusCode.OK,result);
            }
            catch (AppObjectNotFoundException)
            {
                return StatusCode(HttpStatusCode.NotFound);
            }
            catch(DuplicateEntryException)
            {
                return StatusCode(HttpStatusCode.PreconditionFailed);
            }
        }

        /// <summary>
        /// Adds a student to waiting list of specific course given by ID
        /// </summary>
        /// <param name="id">ID of the course</param>
        /// <param name="model">A model containing the SSN of the student</param>
        /// <returns>A personDTO of the student</returns>
        [HttpPost]
        [Route("{id}/waitinglist")]
        [ResponseType(typeof(StudentDTO))]
        public IHttpActionResult AddStudentToWaitingList(int id, AddStudentViewModel model)
        {
            if(ModelState.IsValid)
            {
                try
                {
                    var result = _service.AddStudentToWaitingList(id, model);
                    return Content(HttpStatusCode.OK, result);
                }
                catch (AppObjectNotFoundException)
                {
                    return StatusCode(HttpStatusCode.NotFound);
                }
                catch(DuplicateEntryException)
                {
                    return StatusCode(HttpStatusCode.PreconditionFailed);
                }
            }
            else
            {
                return StatusCode(HttpStatusCode.PreconditionFailed);
            }
        }
        /// <summary>
        /// Removes student from a course given by cid
        /// </summary>
        /// <param name="cid">id of the course</param>
        /// <param name="ssn">ssn of the student</param>
        /// <returns></returns>
        [HttpDelete]
        [Route("{cid}/students/{ssn}")]
        public IHttpActionResult RemoveStudentFromCourse(int cid, string ssn)
        {
            try
            {
                _service.RemoveStudentFromCourse(cid, ssn);
                return StatusCode(HttpStatusCode.NoContent);
            }
            catch (AppObjectNotFoundException)
            {

                return StatusCode(HttpStatusCode.NotFound);
            }
        }
    }
}