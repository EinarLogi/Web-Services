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
using API.Services.Exceptions;

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
                                  on c.TemplateID equals ct.TemplateID
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
        /// adds a new instance of a course to the database
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public CourseDTO AddNewCourse(CourseViewModel model)
        {
            //check if course already in database
          
            //add the course
            var newCourse = new Course
            {

                TemplateID = model.TemplateID,
                Semester = model.Semester,
                StartDate = model.StartDate,
                EndDate = model.EndDate,
                MaxStudents = model.MaxStudents
            };

            _db.Courses.Add(newCourse);
            _db.SaveChanges();

            //find the Name of the course
            var courseTemplate = (from ct in _db.CourseTemplates
                              where ct.TemplateID == model.TemplateID
                              select ct).SingleOrDefault();

            var result = new CourseDTO
            {
                ID = newCourse.ID,
                Name = courseTemplate.Name,
                StartDate = newCourse.StartDate,
                StudentCount = 0
            };

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

            //count students in the course
            var countStudents = _db.CourseStudents.Count(x => x.CourseID == course.ID);

            //stop adding new students if course is full
            if(countStudents >= course.MaxStudents)
            {
                throw new MaxStudentException();
            }

            //check if student already in course
            var studentAlreadyInCourse = (from cs in _db.CourseStudents
                                         join p in _db.Persons on cs.PersonID equals person.ID
                                         where cs.CourseID == id
                                         select cs).SingleOrDefault();

            if (studentAlreadyInCourse != null)
            {
                throw new DuplicateEntryException();
            }


            //check if person is on the waitinglist
            var isOnWaitList = (from cwl in _db.CourseWaitingList
                                join p in _db.Persons on cwl.PersonID equals p.ID
                                where cwl.CourseID == id
                                select cwl).SingleOrDefault();   

            //person is on the waitinglist
            if(isOnWaitList != null)
            {
                _db.CourseWaitingList.Remove(isOnWaitList);
                _db.SaveChanges();
            }

            //Actually add the record
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
            var courseTemplate = _db.CourseTemplates.SingleOrDefault(x => x.TemplateID == courseEntity.TemplateID);
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
            // Validate
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
                                  on c.TemplateID equals ct.TemplateID
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

        /// <summary>
        /// Returns a list of active students on the waiting list
        /// </summary>
        /// <param name="id">ID of the course</param>
        /// <returns>Active students on the waiting list.</returns>
        public CourseWaitingListDTO GetCourseWaitingList(int id)
        {
            // Validate
            var courseEntity = _db.Courses.SingleOrDefault(x => x.ID == id);
            if (courseEntity == null)
            {
                throw new AppObjectNotFoundException();
            }

            var listOfWaitingStudents = (from p in _db.Persons
                                  join cwl in _db.CourseWaitingList
                                          on id equals cwl.CourseID
                                  where p.ID == cwl.PersonID
                                  select new StudentDTO
                                  {
                                      Name = p.Name,
                                      SSN = p.SSN
                                  }).ToList();

            var result = new CourseWaitingListDTO();
            result.WaitingStudents = listOfWaitingStudents;

            return result;

        }

        public void RemoveStudentFromCourse(int cid, string ssn)
        {
            //find the person
            var person = (from p in _db.Persons
                         where p.SSN == ssn
                         select p).SingleOrDefault();

            //check if student is in course
            var studentInCourse = (from cs in _db.CourseStudents
                                   //join p in _db.Persons on cs.PersonID equals person.ID
                                   where cs.CourseID == cid
                                   && person.ID == cs.PersonID
                                   select cs).SingleOrDefault();

            //student not in course
            if(studentInCourse == null)
            {
                throw new AppObjectNotFoundException();
            }
            _db.CourseStudents.Remove(studentInCourse);
            _db.SaveChanges();
  
        }
    }
}
