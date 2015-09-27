using System.Web.Http;

using CoursesAPI.Models;
using CoursesAPI.Services.DataAccess;
using CoursesAPI.Services.Services;
using System.Net.Http;
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

        /// <summary>
        /// Returns a page of courses for a requested page number
        /// </summary>
        /// <param name="semester">The courses semester</param>
        /// <param name="page">The requested page</param>
        /// <returns>An envelope with info and list of courses</returns>
        [HttpGet]
        [AllowAnonymous]
        public IHttpActionResult GetCoursesBySemester(string semester = null, int page = 1)
        {
            // Get the language from the Accept-Header
            var language = Request.Headers.AcceptLanguage.ToString();
            
            var result = _service.GetCourseInstancesBySemester(language, semester, page);
            return Content(HttpStatusCode.OK, result);
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
	}
}
