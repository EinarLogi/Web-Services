using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace API.Services.Entities
{
    [Table("CourseTemplates")]
    class CourseTemplate
    {
        /// <summary>
        /// Database-generated ID of the record
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// The ID of the course.
        /// Example: "T-514-VEFT"
        /// </summary>
        public string TemplateID { get; set; }

        /// <summary>
        /// The name of the course.
        /// Example: "Vefþjónustur"
        /// </summary>
        public string Name { get; set; }
    }
}
