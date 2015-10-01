using System.Web.Http;
using CoursesAPI.Models;
using CoursesAPI.Services.DataAccess;
using CoursesAPI.Services.Services;
using System.Web.Http.Description;
using CoursesAPI.Services.Exceptions;
using System.Net;
using WebApi.OutputCache.V2;
using API.Models;

namespace CoursesAPI.Controllers
{
	[RoutePrefix("api/courses")]
	public class CoursesController : ApiController
	{
		private readonly CoursesServiceProvider _service;

		public CoursesController()
		{
			_service = new CoursesServiceProvider(new UnitOfWork<AppDataContext>());
		}

        /// <summary>
        /// Returns list of courses to everyone
        /// and caches the result for one day
        /// </summary>
        /// <param name="semester">The semesterID of the courses</param>
        /// <param name="page">What page</param>
        /// <returns></returns>
		[HttpGet]
		[AllowAnonymous]
        [CacheOutput(ClientTimeSpan = 86400, ServerTimeSpan = 86400)]
        [Route("")]
		public IHttpActionResult GetCoursesBySemester(string semester = null, int page = 1)
		{
			return Ok(_service.GetCourseInstancesBySemester(semester, page));
		}

        /// <summary>
        /// Adds a new course to the database and 
        /// invalidate the previosly cached result.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        [InvalidateCacheOutput("GetCoursesBySemester")]
        [Route("")]
        [ResponseType(typeof(CourseInstanceDTO))]
        public IHttpActionResult AddNewCourse(CourseViewModel model)
        {
            return StatusCode(HttpStatusCode.Created);
        }

        /// <summary>
        /// For adding a teacher in the database
        /// </summary>
        /// <param name="id"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
		[Route("{id}/teachers")]
		public IHttpActionResult AddTeacher(int id, AddTeacherViewModel model)
		{
			var result = _service.AddTeacherToCourse(id, model);
			return Created("TODO", result);
		}

        /// <summary>
        /// Returns a course with the requested ID
        /// </summary>
        /// <param name="id">ID of the course</param>
        /// <returns></returns>
        [HttpGet]
        [Authorize]
        [Route("{id}", Name = "GetCourseById")]
        [ResponseType(typeof(CourseInstanceDTO))]
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
	}
}
