using System.Collections.Generic;
using System.Linq;
using CoursesAPI.Models;
using CoursesAPI.Services.Exceptions;
using CoursesAPI.Services.Models.Entities;
using CoursesAPI.Services.Services;
using CoursesAPI.Tests.MockObjects;
using CoursesAPI.Tests.TestExtensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CoursesAPI.Tests.Services
{
	[TestClass]
	public class CourseServicesTests
	{
		private MockUnitOfWork<MockDataContext> _mockUnitOfWork;
		private CoursesServiceProvider _service;
		private List<TeacherRegistration> _teacherRegistrations;

		private const string SSN_DABS    = "1203735289";
		private const string SSN_GUNNA   = "1234567890";
        private const string SSN_HRAFN = "9876543210";
		private const string INVALID_SSN = "9876543210";

		private const string NAME_GUNNA  = "Guðrún Guðmundsdóttir";

		private const int COURSEID_VEFT_20153 = 1337;
		private const int COURSEID_VEFT_20163 = 1338;
		private const int INVALID_COURSEID    = 9999;
        private const int COURSEID_PROG_20163 = 1339;

		[TestInitialize]
		public void Setup()
		{
			_mockUnitOfWork = new MockUnitOfWork<MockDataContext>();

			#region Persons
			var persons = new List<Person>
			{
				// Of course I'm the first person,
				// did you expect anything else?
				new Person
				{
					ID    = 1,
					Name  = "Daníel B. Sigurgeirsson",
					SSN   = SSN_DABS,
					Email = "dabs@ru.is"
				},
				new Person
				{
					ID    = 2,
					Name  = NAME_GUNNA,
					SSN   = SSN_GUNNA,
					Email = "gunna@ru.is"
				},
                new Person
                {
                    ID      = 3,
                    Name    = "Hrafn Loftsson",
                    SSN     = SSN_HRAFN,
                    Email   = "hrafn@krummi.is"
                }
			};
			#endregion

			#region Course templates

			var courseTemplates = new List<CourseTemplate>
			{
				new CourseTemplate
				{
					CourseID    = "T-514-VEFT",
					Description = "Í þessum áfanga verður fjallað um vefþj...",
					Name        = "Vefþjónustur"
				},
                new CourseTemplate
                {
                    CourseID    = "T-111-PROG",
                    Description = "Fyrstu skref í forritun...",
                    Name        = "Forritun"
                }
			};
            #endregion

            #region Courses
            var courses = new List<CourseInstance>
            {
                new CourseInstance
                {
                    ID         = COURSEID_VEFT_20153,
                    CourseID   = "T-514-VEFT",
                    SemesterID = "20153"
                },
                new CourseInstance
                {
                    ID         = COURSEID_VEFT_20163,
                    CourseID   = "T-514-VEFT",
                    SemesterID = "20163"
                },
                new CourseInstance
                {
                    ID = COURSEID_PROG_20163,
                    CourseID = "T-111-PROG",
                    SemesterID = "20163"
                }
			};
			#endregion

			#region Teacher registrations
			_teacherRegistrations = new List<TeacherRegistration>
			{
				new TeacherRegistration
				{
					ID               = 101,
					CourseInstanceID = COURSEID_VEFT_20153,
					SSN              = SSN_DABS,
					Type             = TeacherType.MainTeacher
				},
                new TeacherRegistration
                {
                    ID               = 102,
                    CourseInstanceID = COURSEID_PROG_20163,
                    SSN              = SSN_HRAFN,
                    Type             = TeacherType.MainTeacher
                }
			};
			#endregion

			_mockUnitOfWork.SetRepositoryData(persons);
			_mockUnitOfWork.SetRepositoryData(courseTemplates);
			_mockUnitOfWork.SetRepositoryData(courses);
			_mockUnitOfWork.SetRepositoryData(_teacherRegistrations);

			// TODO: this would be the correct place to add 
			// more mock data to the mockUnitOfWork!

			_service = new CoursesServiceProvider(_mockUnitOfWork);
		}

		#region GetCoursesBySemester
		/// <summary>
		/// Unit test for course that doesn't yet have a registered semester.
        /// Should return empty list
		/// </summary>
		[TestMethod]
		public void GetCoursesBySemester_ReturnsEmptyListWhenNoDataDefined()
		{
            // Arrange:
            var service = _service;

            // Act:
            var result = service.GetCourseInstancesBySemester("20151");

            // Assert:
            Assert.AreEqual(0,result.Count, "The number of courses are incorrect");
		}

		/// <summary>
        /// Tests when no argument is given.
        /// function should return courses for 20153 when no argument is given.
        /// </summary>
        [TestMethod]
        public void GetCoursesBySemester_Returns20153CoursesWhenNoArgumentIsGiven()
        {
            //Arrange:
            var service = _service;

            //Act:
            var result = service.GetCourseInstancesBySemester("");

            //Assert:
            Assert.AreEqual(1, result.Count, "The number of courses are incorrect");

            var courseInstanceDTO = result[0];

            //check if DTO id is identical to the id of the returned object
            Assert.AreEqual(COURSEID_VEFT_20153, courseInstanceDTO.CourseInstanceID);

            //check if correct main teacher is registered
            Assert.AreEqual("Daníel B. Sigurgeirsson", courseInstanceDTO.MainTeacher, "Dabs should teach this course");
            
        }
        /// <summary>
        /// Tests input which doesn't translate to a semester.
        /// Should return empty list.
        /// </summary>
        [TestMethod]
        public void GetCoursesOnIlligalInput_ReturnsEmptyList()
        {
            //Arrange:
            var service = _service;

            //Act:
            var result = service.GetCourseInstancesBySemester("abc");

            // Assert:

            //no course should be returned.
            Assert.AreEqual(0, result.Count, "the number of courses is incorrect");
        }
        /// <summary>
        /// Tests for input 20153
        /// </summary>
        [TestMethod]
        public void GetCoursesBySemester_ReturnsListOfCoursesWithSemesterEqualto20153()
        {
            //Arrange:
            var service = _service;

            //Act:
            var result = service.GetCourseInstancesBySemester("20153");

            //Assert:
       
            //check if one course is returned
            Assert.AreEqual(1, result.Count, "The number of courses is incorrect");
        }

        /// <summary>
        /// Gets courses from semester 20163
        /// </summary>
        [TestMethod]
        public void GetCoursesBySemester_ReturnsListOfCoursesWithSemesterEqualto20163()
        {
            //Arrange:
            var service = _service;

            //Act:
            var result = service.GetCourseInstancesBySemester("20163");

            //Assert:

            //2 courses should be registered for 20163
            Assert.AreEqual(2, result.Count, "The number of courses is incorrect");

            //make sure no main teacher is registered for vefþjónustur 20163
            var courseInstanceDTO = result[0];
            Assert.AreEqual("", courseInstanceDTO.MainTeacher, "No main teacher should be teaching this course");

            //make sure main teacher is registered for forritun 20163
            var secondCourseInstanceDTO = result[1];
            Assert.AreEqual("Hrafn Loftsson", secondCourseInstanceDTO.MainTeacher, "Hrafn Loftsson should be the main teacher");
        }

        #endregion

        #region AddTeacher

        /// <summary>
        /// Adds a main teacher to a course which doesn't have a
        /// main teacher defined already (see test data defined above).
        /// </summary>
        [TestMethod]
		public void AddTeacher_WithValidTeacherAndCourse()
		{
			// Arrange:
			var model = new AddTeacherViewModel
			{
				SSN  = SSN_GUNNA,
				Type = TeacherType.MainTeacher
			};
			var prevCount = _teacherRegistrations.Count;
			// Note: the method uses test data defined in [TestInitialize]

			// Act:
			var dto = _service.AddTeacherToCourse(COURSEID_VEFT_20163, model);

			// Assert:

			// Check that the dto object is correctly populated:
			Assert.AreEqual(SSN_GUNNA, dto.SSN, "Inconsistent dto.SSN");
			Assert.AreEqual(NAME_GUNNA, dto.Name, "Inconsistent dto.name");

			// Ensure that a new entity object has been created:
			var currentCount = _teacherRegistrations.Count;
			Assert.AreEqual(prevCount + 1, currentCount, "Object has not been created");

			// Get access to the entity object and assert that
			// the properties have been set:
			var newEntity = _teacherRegistrations.Last();
			Assert.AreEqual(COURSEID_VEFT_20163, newEntity.CourseInstanceID, "Inconsistent newEntity.CourseInstanceID");
			Assert.AreEqual(SSN_GUNNA, newEntity.SSN, "Inconsistent newEntity.SSN");
			Assert.AreEqual(TeacherType.MainTeacher, newEntity.Type, "Inconsistent newEntity.Type");

			// Ensure that the Unit Of Work object has been instructed
			// to save the new entity object:
			Assert.IsTrue(_mockUnitOfWork.GetSaveCallCount() > 0, "GetSaveCallCount() <= 0");
		}

		[TestMethod]
		[ExpectedException(typeof(AppObjectNotFoundException))]
		public void AddTeacher_InvalidCourse()
		{
			// Arrange:
			var model = new AddTeacherViewModel
			{
				SSN  = SSN_GUNNA,
				Type = TeacherType.AssistantTeacher
			};
			// Note: the method uses test data defined in [TestInitialize]

			// Act:
			_service.AddTeacherToCourse(INVALID_COURSEID, model);
		}

		/// <summary>
		/// Ensure it is not possible to add a person as a teacher
		/// when that person is not registered in the system.
		/// </summary>
		[TestMethod]
		[ExpectedException(typeof (AppObjectNotFoundException))]
		public void AddTeacher_InvalidTeacher()
		{
			// Arrange:
			var model = new AddTeacherViewModel
			{
				SSN  = INVALID_SSN,
				Type = TeacherType.MainTeacher
			};
			// Note: the method uses test data defined in [TestInitialize]

			// Act:
			_service.AddTeacherToCourse(COURSEID_VEFT_20153, model);
		}

		/// <summary>
		/// In this test, we test that it is not possible to
		/// add another main teacher to a course, if one is already
		/// defined.
		/// </summary>
		[TestMethod]
		[ExpectedExceptionWithMessage(typeof (AppValidationException), "COURSE_ALREADY_HAS_A_MAIN_TEACHER")]
		public void AddTeacher_AlreadyWithMainTeacher()
		{
			// Arrange:
			var model = new AddTeacherViewModel
			{
				SSN  = SSN_GUNNA,
				Type = TeacherType.MainTeacher
			};
			// Note: the method uses test data defined in [TestInitialize]

			// Act:
			_service.AddTeacherToCourse(COURSEID_VEFT_20153, model);
		}

		/// <summary>
		/// In this test, we ensure that a person cannot be added as a
		/// teacher in a course, if that person is already registered
		/// as a teacher in the given course.
		/// </summary>
		[TestMethod]
		[ExpectedExceptionWithMessage(typeof (AppValidationException), "PERSON_ALREADY_REGISTERED_TEACHER_IN_COURSE")]
		public void AddTeacher_PersonAlreadyRegisteredAsTeacherInCourse()
		{
			// Arrange:
			var model = new AddTeacherViewModel
			{
				SSN  = SSN_DABS,
				Type = TeacherType.AssistantTeacher
			};
			// Note: the method uses test data defined in [TestInitialize]

			// Act:
			_service.AddTeacherToCourse(COURSEID_VEFT_20153, model);
		}

		#endregion
	}
}
