using CRUDTestProject.Data;
using CRUDTestProject.Data.Entities;
using CRUDTestProject.Models;
using CRUDTestProject.Models.Response;
using Microsoft.AspNetCore.Http;
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
        const string USERNAME = "1u";
        const string EMAIL = "1e";
        const string CONTENT = "1c";
        const string NAME = "1n";
        private Guid ID;

        private readonly HttpClient client;
        private readonly CustomWebApplicationFactory factory;

        private List<Message> getMessages()
        {
            return
            [
                new (){ Content = CONTENT, CreationDate = DateTime.Now.AddDays(-1), Email = EMAIL, Name = NAME, Username = USERNAME},
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
                context.RemoveRange(context.Messages);
                context.UpdateRange(getMessages());
                context.SaveChanges();

                ID = context.Messages.Where(m => m.Username == USERNAME).FirstOrDefault().Id;
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

        [Fact]
        public async Task GetMessagesFilteringUsername()
        {

            var response = await client.GetAsync("/api/messages?matchUsername=" + USERNAME);
            response.EnsureSuccessStatusCode();
            var result = await response.Content.ReadAsStringAsync();
            var messageList = JsonConvert.DeserializeObject<List<MessageResponseModel>>(result);

            Assert.Equal(USERNAME, messageList.First().Username);
        }

        [Fact]
        public async Task GetMessagesFilteringContent()
        {

            var response = await client.GetAsync("/api/messages?SearchInContent=" + CONTENT);
            response.EnsureSuccessStatusCode();
            var result = await response.Content.ReadAsStringAsync();
            var messageList = JsonConvert.DeserializeObject<List<MessageResponseModel>>(result);

            Assert.Equal(CONTENT, messageList.Last().Content);
        }

        [Fact]
        public async Task GetMessagesFilteringName()
        {

            var response = await client.GetAsync("/api/messages?SearchInName=" + NAME);
            response.EnsureSuccessStatusCode();
            var result = await response.Content.ReadAsStringAsync();
            var messageList = JsonConvert.DeserializeObject<List<MessageResponseModel>>(result);

            Assert.Equal(NAME, messageList.Last().Name);
        }

        private string? GetToken(string username)
        {
            List<Claim> authClaims = new List<Claim>() { new Claim(ClaimTypes.Name, username), new Claim(ClaimTypes.Email, EMAIL) };
            var configuration = factory.Services.CreateScope().ServiceProvider.GetRequiredService<IConfiguration>();
            var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JWT:Secret"]));

            var token = new JwtSecurityToken(
                issuer: configuration["JWT:Issuer"],
                audience: configuration["JWT:Audience"],
                expires: DateTime.Now.AddHours(1),
                claims: authClaims,
                signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
                );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        [Fact]
        public async Task PostMessageWhenLoggedIn()
        {

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", GetToken(USERNAME));
            var toPost = new AddMessageDto { Content = "content", Name = "name" };
            var response = await client.PostAsJsonAsync("/api/messages", toPost);
            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadAsStringAsync();
            var message = JsonConvert.DeserializeObject<MessageResponseModel>(result);

            Assert.Equal(toPost.Content, message.Content);
            Assert.Equal(toPost.Name, message.Name);
            Assert.Equal(USERNAME, message.Username);
        }

        [Fact]
        public async Task PostMessageWhenNotLoggedIn()
        {
            var toPost = new AddMessageDto { Content = "content", Name = "name" };
            var response = await client.PostAsJsonAsync("/api/messages", toPost);

            Assert.Equal(StatusCodes.Status401Unauthorized, ((int)response.StatusCode));
        }

        [Fact]
        public async Task UpdateYourMessageWhenLoggedIn()
        {
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", GetToken(USERNAME));
            var toPost = new UpdateMessageDto { Content = "updatedContent", Name = "updatedName" };
            var response = await client.PutAsJsonAsync("/api/messages/" + ID, toPost);
            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadAsStringAsync();
            var message = JsonConvert.DeserializeObject<MessageResponseModel>(result);

            Assert.Equal(toPost.Content, message.Content);
            Assert.Equal(toPost.Name, message.Name);
            Assert.Equal(USERNAME, message.Username);
        }

        [Fact]
        public async Task UpdateNotYourMessageWhenLoggedIn()
        {
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", GetToken(USERNAME + "notEmpty"));
            var toPost = new UpdateMessageDto { Content = "updatedContent", Name = "updatedName" };
            var response = await client.PutAsJsonAsync("/api/messages/" + ID, toPost);

            Assert.Equal(StatusCodes.Status401Unauthorized, ((int)response.StatusCode));
        }

        [Fact]
        public async Task UpdateMessageWhenNotLoggedIn()
        {
            var toPost = new UpdateMessageDto { Content = "updatedContent", Name = "updatedName" };
            var response = await client.PutAsJsonAsync("/api/messages/" + ID, toPost);

            Assert.Equal(StatusCodes.Status401Unauthorized, ((int)response.StatusCode));
        }

        [Fact]
        public async Task GetMessageWithExistingId()
        {
            var response = await client.GetAsync("/api/messages/" + ID);
            response.EnsureSuccessStatusCode();
            var result = await response.Content.ReadAsStringAsync();
            var message = JsonConvert.DeserializeObject<MessageResponseModel>(result);

            Assert.Equal(ID, message.Id);
        }

        [Fact]
        public async Task GetMessageWithMissingId()
        {
            var response = await client.GetAsync("/api/messages/" + Guid.Empty);

            Assert.Equal(StatusCodes.Status404NotFound, ((int)response.StatusCode));
        }

        [Fact]
        public async Task DeleteYourMessageWhenLoggedIn()
        {
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", GetToken(USERNAME));
            var response = await client.DeleteAsync("/api/messages/" + ID);
            response.EnsureSuccessStatusCode();
        }

        [Fact]
        public async Task DeleteNotYourMessageWhenLoggedIn()
        {
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", GetToken(USERNAME + "notEmpty"));
            var response = await client.DeleteAsync("/api/messages/" + ID);

            Assert.Equal(StatusCodes.Status401Unauthorized, ((int)response.StatusCode));
        }

        [Fact]
        public async Task DeleteMessageWhenNotLoggedIn()
        {
            var response = await client.DeleteAsync("/api/messages/" + ID);

            Assert.Equal(StatusCodes.Status401Unauthorized, ((int)response.StatusCode));
        }
    }
}
