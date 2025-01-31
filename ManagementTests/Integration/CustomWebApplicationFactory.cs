using Management.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System.Data.Common;

namespace ManagementTests.Integration
{
    public class CustomWebApplicationFactory : WebApplicationFactory<Program>
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            base.ConfigureWebHost(builder);

            builder.ConfigureTestServices(services =>
            {

                services.RemoveAll(typeof(DbContextOptions<ManagementDbContext>));
                services.RemoveAll(typeof(DbConnection));

                services.AddDbContext<ManagementDbContext>(options =>
                    options.UseInMemoryDatabase("TestDb"));
            });
        }
    }
}
