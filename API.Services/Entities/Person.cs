using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace API.Services.Entities
{
    [Table("Persons")]
    class Person
    {
        /// <summary>
        /// Database generated unique ID
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// The SSN of the person.
        /// Example: "0202893109"
        /// </summary>
        public string SSN { get; set; }

        /// <summary>
        /// The name of the person.
        /// Example: "Sigurður Gunnar Jónsson"
        /// </summary>
        public string Name { get; set; }
    }
}
