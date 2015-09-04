using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace API.Services.Entities
{
    //[Table("Courses")]
    class Course
    {
        /// <summary>
        /// 
        /// </summary>
        public int ID { get; set; }

        //public string Name { get; set; }

        /// <summary>
        /// An identifier for the template course
        /// Example: "T-514-VEFT"
        /// </summary>
        public string CourseIdentifer { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public DateTime StartDate { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public DateTime EndDate { get; set; }

        public String Semester { get; set; }
    }
}
