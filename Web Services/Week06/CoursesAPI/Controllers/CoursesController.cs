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

        /*[HttpGet]
		[AllowAnonymous]
		public IHttpActionResult GetCoursesBySemester(string semester = null, int page = 1)
		{
            // TODO: figure out the requested language (if any!)
            // and pass it to the service provider!
            return Ok(_service.GetCourseInstancesBySemester(semester, page));
		}*/

        [HttpGet]
        [AllowAnonymous]
        public IHttpActionResult GetCoursesBySemester(string semester = null, int page = 1)
        {
            var language = Request.Headers.AcceptLanguage.ToString();
            // TODO: figure out the requested language (if any!)
            // and pass it to the service provider!
            if (ModelState.IsValid)
            {
                //HttpError error = GetErrors(ModelState, true);
                //return Request.CreateResponse(HttpStatusCode.BadRequest, error);
            }
            //return Ok(_service.GetCourseInstancesBySemester(semester, page));
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
