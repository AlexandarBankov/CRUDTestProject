using Management.Data.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Management.Data
{
    public class ManagementDbContext : IdentityDbContext<User>
    {
        public ManagementDbContext(DbContextOptions<ManagementDbContext> options) 
            : base(options)
        { }
    }
}
