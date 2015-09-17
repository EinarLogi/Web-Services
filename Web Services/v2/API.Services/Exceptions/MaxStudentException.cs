using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace API.Services.Exceptions
{
    /// <summary>
    /// An instance of this class will be thrown if the course has reached maxStudent count
    /// </summary>
    public class MaxStudentException : ApplicationException
    {
    }
}
