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
        /// <param name="id"></param>
        public void DeleteCourseById(int id)
        {
            var course = _db.Courses.SingleOrDefault(x  => x.ID == id);

            //course not in database
            if(course == null)
            {
                throw new AppObjectNotFoundException();
            }

            var courseTemplate = (from ct in _db.CourseTemplates
                         where ct.CourseID == course.CourseIdentifier select ct).Single();


            _db.Courses.Remove(course);
            _db.CourseTemplates.Remove(courseTemplate);


        }

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

        public CourseDTO UpdateCourse(int id, CourseUpdateViewModel model)
        {
            //1. Validate
            //2. Update
            var courseEntity = _db.Courses.SingleOrDefault(x => x.ID == id);
            if(courseEntity == null)
            {
                throw new AppObjectNotFoundException();
            }
            //make changes to the entity object and save changes to database
            courseEntity.StartDate = model.StartDate;
            courseEntity.EndDate = model.EndDate;

            _db.SaveChanges();
            //3.Return
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
    }
}
