using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace API.Services.Exception
{
    /// <summary>
    /// An instance of this class will be thrown if an object
    /// (such as a course, or a student in course) cannot be found.
    /// </summary>
    public class AppObjectNotFoundException : ApplicationException
    {
    }
}
