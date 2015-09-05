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
        /// 
        /// </summary>
        public int ID { get; set; }

        public string CourseID { get; set; }

        public string Name { get; set; }
    }
}
