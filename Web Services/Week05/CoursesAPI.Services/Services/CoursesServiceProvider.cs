using System.Collections.Generic;
using System.Linq;
using CoursesAPI.Models;
using CoursesAPI.Services.DataAccess;
using CoursesAPI.Services.Exceptions;
using CoursesAPI.Services.Models.Entities;

namespace CoursesAPI.Services.Services
{
	public class CoursesServiceProvider
	{
		private readonly IUnitOfWork _uow;

		private readonly IRepository<CourseInstance> _courseInstances;
		private readonly IRepository<TeacherRegistration> _teacherRegistrations;
		private readonly IRepository<CourseTemplate> _courseTemplates; 
		private readonly IRepository<Person> _persons;

		public CoursesServiceProvider(IUnitOfWork uow)
		{
			_uow = uow;

			_courseInstances      = _uow.GetRepository<CourseInstance>();
			_courseTemplates      = _uow.GetRepository<CourseTemplate>();
			_teacherRegistrations = _uow.GetRepository<TeacherRegistration>();
			_persons              = _uow.GetRepository<Person>();
		}

		/// <summary>
		/// You should implement this function, such that all tests will pass.
		/// </summary>
		/// <param name="courseInstanceID">The ID of the course instance which the teacher will be registered to.</param>
		/// <param name="model">The data which indicates which person should be added as a teacher, and in what role.</param>
		/// <returns>Should return basic information about the person.</returns>
		public PersonDTO AddTeacherToCourse(int courseInstanceID, AddTeacherViewModel model)
		{
            var course = (from c in _courseInstances.All()
                          where c.ID == courseInstanceID
                          select c).SingleOrDefault();
            // Make sure that the course exists
            if (course == null)
            {
                throw new AppObjectNotFoundException();
            }

            var person = (from p in _persons.All()
                          where p.SSN == model.SSN
                          select p).SingleOrDefault();
            // Make sure that the person exists
            if (person == null)
            {
                throw new AppObjectNotFoundException();
            }

            if(model.Type == TeacherType.MainTeacher)
            {
                // Check if the course aldready has a main teacher
                var teacherReg = (from tr in _teacherRegistrations.All()
                                  where tr.CourseInstanceID == course.ID
                                        && tr.Type == TeacherType.MainTeacher
                                  select tr).SingleOrDefault();
                if(teacherReg != null)
                {
                    throw new AppValidationException("COURSE_ALREADY_HAS_A_MAIN_TEACHER");
                }
            }

            // Check if the person is already a teacher in the course
            var alreadyTeacher = (from tr in _teacherRegistrations.All()
                                  where tr.CourseInstanceID == course.ID
                                        && tr.SSN == person.SSN
                                  select tr).SingleOrDefault();
            if(alreadyTeacher != null)
            {
                throw new AppValidationException("PERSON_ALREADY_REGISTERED_TEACHER_IN_COURSE");
            }

            // Create TeacerRegistration entity
            var teacherRegistration = new TeacherRegistration
            {
                SSN = person.SSN,
                CourseInstanceID = course.ID,
                Type = model.Type
            };

            // Add to database
            _teacherRegistrations.Add(teacherRegistration);
            _uow.Save();

            var result = new PersonDTO
            {
                SSN = model.SSN,
                Name = person.Name
            };

            return result;
		}

		/// <summary>
		/// You should write tests for this function. You will also need to
		/// modify it, such that it will correctly return the name of the main
		/// teacher of each course.
		/// </summary>
		/// <param name="semester"></param>
		/// <returns></returns>
		public List<CourseInstanceDTO> GetCourseInstancesBySemester(string semester = null)
		{
			if (string.IsNullOrEmpty(semester))
			{
				semester = "20153";
			}

			var courses = (from c in _courseInstances.All()
				join ct in _courseTemplates.All() on c.CourseID equals ct.CourseID
				where c.SemesterID == semester
				select new CourseInstanceDTO
				{
					Name               = ct.Name,
					TemplateID         = ct.CourseID,
					CourseInstanceID   = c.ID,
					MainTeacher        = "" // Hint: it should not always return an empty string!
				}).ToList();

			return courses;
		}
	}
}
