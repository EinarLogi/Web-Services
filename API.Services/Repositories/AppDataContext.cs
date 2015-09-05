using API.Services.Entities;
using System.Data.Entity;

namespace API.Services.Repositories
{
    class AppDataContext : DbContext
    {
        public DbSet<Course> Courses { get; set; }

        public DbSet<CourseTemplate> CourseTemplates { get; set; }
    }
}