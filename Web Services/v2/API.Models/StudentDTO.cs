using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace API.Models
{
    /// <summary>
    /// This class represents a single student
    /// </summary>
    public class StudentDTO
    {
        /// <summary>
        /// Name of the person
        /// Example: "Sigurður Gunnar Jónsson"
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// SSN of the person
        /// Example: "0202893109"
        /// </summary>
        public string SSN { get; set; }

    }
}
