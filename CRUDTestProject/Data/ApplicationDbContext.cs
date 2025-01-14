using CRUDTestProject.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace CRUDTestProject.Data
{
    public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
    {
        public DbSet<Message> Messages { get; set; }
    }
}
