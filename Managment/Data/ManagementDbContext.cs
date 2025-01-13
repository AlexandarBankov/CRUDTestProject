using Management.Data.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Management.Data
{
    public class ManagementDbContext(DbContextOptions<ManagementDbContext> options) : IdentityDbContext<User>(options)
    {
    }
}
