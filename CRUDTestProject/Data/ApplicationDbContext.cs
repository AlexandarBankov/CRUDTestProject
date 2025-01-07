using CRUDTestProject.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace CRUDTestProject.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {   
        }

        public DbSet<Message> Messages { get; set; }
    }
}
