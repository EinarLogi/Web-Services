using System.Web.Http;
using CoursesAPI.Models;
using CoursesAPI.Services.DataAccess;
using CoursesAPI.Services.Services;
using System.Web.Http.Description;
using CoursesAPI.Services.Exceptions;
using System.Net;

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

		[HttpGet]
		[AllowAnonymous]
        [Route("")]
		public IHttpActionResult GetCoursesBySemester(string semester = null, int page = 1)
		{
			// TODO: figure out the requested language (if any!)
			// and pass it to the service provider!
			return Ok(_service.GetCourseInstancesBySemester(semester, page));
		}

		/// <summary>
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
        /// 
        /// </summary>
        /// <param name="id"></param>
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
