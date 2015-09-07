using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using API.Models;
using API.Services.Repositories;
using API.Models.Courses.Students;
using API.Services.Exception;
using API.Services.Entities;
using API.Models.Courses;

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
                            StudentCount    = 0 
                           }).ToList();

            return result;
        }

        /// <summary>
        /// Deletes a course from Courses and CourseTemplates.
        /// Carfeul.
        /// </summary>
        /// <param name="id">The id of the course to be deleted</param>
        public void DeleteCourseById(int id)
        {
            var course = _db.Courses.SingleOrDefault(x  => x.ID == id);

            //course not in database
            if(course == null)
            {
                throw new AppObjectNotFoundException();
            }

            _db.Courses.Remove(course);
            _db.SaveChanges();
        }
        /// <summary>
        /// Returs a list of students in a given course.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public List<StudentDTO> GetStudentsInCourse(int id)
        {
            var course = _db.Courses.SingleOrDefault(x => x.ID == id);

            if (course == null)
            {
                throw new AppObjectNotFoundException();
            }

            var result = (from p in _db.Persons
                          join cs in _db.CourseStudents
                                  on id equals cs.CourseID
                          where p.ID == cs.PersonID
                          select new StudentDTO
                          {
                              Name = p.Name,
                              SSN = p.SSN
                          }).ToList();

            return result;
        }

        /// <summary>
        /// Adds a student to a given course
        /// </summary>
        /// <param name="id"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        public StudentDTO AddStudentToCourse(int id, AddStudentViewModel model)
        {
            
            var course = _db.Courses.SingleOrDefault(x => x.ID == id);
            //if course doesn't exist an error will be thrown. Not possible to add student to non existing courses
            if (course == null)
            {
                throw new AppObjectNotFoundException();
            }

            //verify that the person exists!
            var person = _db.Persons.SingleOrDefault(x => x.SSN == model.SSN);
            if(person == null)
            {
                throw new AppObjectNotFoundException();
            }

            //2. Actually add the record
            var courseStudent = new CourseStudent
            {
                PersonID = person.ID,
                CourseID = course.ID
            };

            _db.CourseStudents.Add(courseStudent);
            _db.SaveChanges();

            //3.Figure out what to return.
            var returnValue = new StudentDTO
            {
                Name = person.Name,
                SSN = person.SSN
            };
            return returnValue;
        }
        /// <summary>
        /// Updates a specific course by a given id.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        public CourseDTO UpdateCourse(int id, CourseUpdateViewModel model)
        {
            // Validate
            var courseEntity = _db.Courses.SingleOrDefault(x => x.ID == id);
            if(courseEntity == null)
            {
                throw new AppObjectNotFoundException();
            }
            //make changes to the entity object and save changes to database
            courseEntity.StartDate = model.StartDate;
            courseEntity.EndDate = model.EndDate;

            if(model.MaxStudents != 0)
            {
                courseEntity.MaxStudents = model.MaxStudents;
            }

            _db.SaveChanges();
            // Return
            var courseTemplate = _db.CourseTemplates.SingleOrDefault(x => x.CourseID == courseEntity.CourseIdentifier);
            if (courseTemplate == null)
            {
                //return 500 internal server error
                throw new ApplicationException("");
            }
            var count = _db.CourseStudents.Count(x => x.CourseID == courseEntity.ID);
            var result = new CourseDTO
            {
                ID = courseEntity.ID,
                Name = courseTemplate.Name,
                StartDate = courseEntity.StartDate,
                StudentCount = count
            };
            return result;
        }

        /// <summary>
        /// Returns CourseDetails for a specific course
        /// </summary>
        /// <param name="id">ID of the requested course</param>
        /// <returns>A CourseDetailsDTO object</returns>
        public CourseDetailsDTO GetCourseById(int id)
        {
            // 1. Validate
            var courseEntity = _db.Courses.SingleOrDefault(x => x.ID == id);
            if (courseEntity == null)
            {
                throw new AppObjectNotFoundException();
            }

            var listOfStudents = (from p in _db.Persons
                                  join cs in _db.CourseStudents
                                          on id equals cs.CourseID
                                  where p.ID == cs.PersonID
                                  select new StudentDTO
                                  {
                                      Name      = p.Name,
                                      SSN       = p.SSN
                                  }).ToList();


            var courseObj = (from c in _db.Courses
                            join ct in _db.CourseTemplates
                                  on c.CourseIdentifier equals ct.CourseID
                            where c.ID == id
                            select new CourseDetailsDTO
                            {
                                ID              = c.ID,
                                Name            = ct.Name,
                                MaxStudents     = c.MaxStudents
                            }).SingleOrDefault();
            // Add listOfStudents to the courseObj
            courseObj.Students = listOfStudents;

            return courseObj;
        }

        /// <summary>
        /// Adds a student to waiting list of specific course given by ID
        /// </summary>
        /// <param name="id">ID of the course</param>
        /// <param name="model">A model containing the SSN of the student</param>
        /// <returns>A personDTO of the student</returns>
        public StudentDTO AddStudentToWaitingList(int id, AddStudentViewModel model)
        {
            // Validate
            var courseEntity = _db.Courses.SingleOrDefault(x => x.ID == id);
            if (courseEntity == null)
            {
                throw new AppObjectNotFoundException();
            }

            //verify that the person exists!
            var person = _db.Persons.SingleOrDefault(x => x.SSN == model.SSN);
            if (person == null)
            {
                throw new AppObjectNotFoundException();
            }

            var waitingEntity = new CourseWaitingList
            {
                CourseID        = id,
                PersonID        = person.ID
            };

            _db.CourseWaitingList.Add(waitingEntity);
            _db.SaveChanges();

            var result = new StudentDTO
            {
                SSN     = model.SSN,
                Name    = person.Name
            };

            return result;
        }
    }
}
