using CRUDTestProject.Data;
using CRUDTestProject.Data.Entities;
using CRUDTestProject.Models;
using CRUDTestProject.Models.Response;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Text;

namespace MessagesTests.Integration
{
    public class MessageApiIntegrationTests : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly HttpClient client;
        private readonly CustomWebApplicationFactory factory;

        private List<Message> getMessages()
        {
            return
            [
                new (){ Content = "1c", CreationDate = DateTime.Now.AddDays(-1), Email = "1e", Name = "1n", Username = "1u"},
                new (){ Content = "2c", CreationDate = DateTime.Now.AddDays(-2), Email = "2e", Name = "2n", Username = "2u"},
                new (){ Content = "3c", CreationDate = DateTime.Now.AddDays(-3), Email = "3e", Name = "3n", Username = "3u"},
                new (){ Content = "4c", CreationDate = DateTime.Now.AddDays(-4), Email = "4e", Name = "4n", Username = "4u"},
                new (){ Content = "5c", CreationDate = DateTime.Now.AddDays(-5), Email = "5e", Name = "5n", Username = "5u"}
            ];
        }

        public MessageApiIntegrationTests(CustomWebApplicationFactory factory)
        {
            client = factory.CreateClient();
            using (var scope = factory.Services.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

                context.UpdateRange(getMessages());
                context.SaveChanges();
            }

            this.factory = factory;
        }

        [Fact]
        public async Task GetMessagesWithoutFilters()
        {
            
            var response = await client.GetAsync("/api/messages");
            response.EnsureSuccessStatusCode();
            var result = await response.Content.ReadAsStringAsync();
            var messageList = JsonConvert.DeserializeObject<List<MessageResponseModel>>(result);

            Assert.Equal(getMessages().Count, messageList.Count);
        }

        private JwtSecurityToken GetToken()
        {
            List<Claim> authClaims = new List<Claim>() { new Claim(ClaimTypes.Name, "1u"), new Claim(ClaimTypes.Email, "1e")};
            var configuration = factory.Services.CreateScope().ServiceProvider.GetRequiredService<IConfiguration>();
            var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JWT:Secret"]));

            var token = new JwtSecurityToken(
                issuer: configuration["JWT:Issuer"],
                audience: configuration["JWT:Audience"],
                expires: DateTime.Now.AddHours(1),
                claims: authClaims,
                signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
                );

            return token;
        }

        [Fact]
        public async Task PostMessageWhenLoggedIn()
        {
            var token = new JwtSecurityTokenHandler().WriteToken(GetToken());

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await client.PostAsJsonAsync("/api/messages", new AddMessageDto() { Content = "content", Name = "name"});
            response.EnsureSuccessStatusCode();
            var result = await response.Content.ReadAsStringAsync();
            var message = JsonConvert.DeserializeObject<MessageResponseModel>(result);

        }
    }
}
