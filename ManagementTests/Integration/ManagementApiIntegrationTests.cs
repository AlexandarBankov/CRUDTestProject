using Management.Data.Entities;
using Management.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Json;
using System.Security.Claims;

namespace ManagementTests.Integration
{
    public class ManagementApiIntegrationTests : IClassFixture<CustomWebApplicationFactory>
    {
        const string USERNAME = "username";
        const string PASSWORD = "password";

        private readonly HttpClient client;
        private readonly CustomWebApplicationFactory factory;
        private readonly int userCount;

        public ManagementApiIntegrationTests(CustomWebApplicationFactory factory)
        {
            client = factory.CreateClient();
            using (var scope = factory.Services.CreateScope())
            {
                var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();

                var Users = userManager.Users.ToList();
                foreach (var user in Users)
                {
                    _ = userManager.DeleteAsync(user).Result;
                }
                var toAdd = new User() { UserName = USERNAME, Email = "", TenantId = "" };
                _ = userManager.CreateAsync(toAdd, PASSWORD).Result;
                userCount = userManager.Users.Count();
            }

            this.factory = factory;
        }

        [Fact]
        public async Task LoginUserWithValidInformation()
        {
            var toPost = new LoginUserModel() { Username = USERNAME, Password = PASSWORD };
            var response = await client.PostAsJsonAsync("/auth/user", toPost);
            response.EnsureSuccessStatusCode();
            var result = await response.Content.ReadAsStringAsync();

            var token = new JwtSecurityTokenHandler().ReadJwtToken(result);

            Assert.Equal(USERNAME, token.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name).Value);
        }

        [Fact]
        public async Task LoginUserWithInvalidInformation()
        {
            var toPost = new LoginUserModel() { Username = Guid.NewGuid().ToString(), Password = Guid.NewGuid().ToString() };
            var response = await client.PostAsJsonAsync("/auth/user", toPost);

            Assert.Equal(StatusCodes.Status401Unauthorized, ((int)response.StatusCode));
        }

        [Fact]
        public async Task CreateUserWithDistinctUsername()
        {
            var toPost = new RegisterUserModel() { Username = Guid.NewGuid().ToString(), Password = PASSWORD, Email = "", TenantId = "" };
            var response = await client.PostAsJsonAsync("/management/user/create", toPost);
            response.EnsureSuccessStatusCode();

            using var scope = factory.Services.CreateScope();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();

            Assert.Equal(userCount + 1, userManager.Users.Count());
            Assert.True(userManager.Users.Any(u => u.UserName == toPost.Username));
        }

        [Fact]
        public async Task CreateUserWithExistingUsername()
        {
            var toPost = new RegisterUserModel() { Username = USERNAME, Password = PASSWORD, Email = "", TenantId = "" };
            var response = await client.PostAsJsonAsync("/management/user/create", toPost);

            Assert.Equal(StatusCodes.Status409Conflict, ((int)response.StatusCode));
        }
    }
}
