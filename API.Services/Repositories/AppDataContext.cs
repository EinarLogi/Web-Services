using API.Services.Entities;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace API.Services.Repositories
{
    class AppDataContext : DbContext
    {
        DbSet<Course> Courses { get; set; }


    }
}
