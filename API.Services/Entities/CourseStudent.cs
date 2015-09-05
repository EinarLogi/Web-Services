using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace API.Services.Entities
{
    [Table("CourseStudents")]
    class CourseStudent
    {
        /// <summary>
        /// Database-generated ID of the record
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// The database-generated ID of the course.
        /// Not null
        /// </summary>
        public int CourseID { get; set; }

        /// <summary>
        /// The database-generated ID of the person which is registered
        /// in the course. Not null.
        /// </summary>
        public int PersonID { get; set; }
    }
}
