using CRUDTestProject.Data;
using CRUDTestProject.Data.Entities;
using CRUDTestProject.Models;
using CRUDTestProject.Models.Response;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using SharedLibrary;
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
        const string BADWORD = "BAAAAAAAAAAAAADDDDDDDDDDDDDDDDDDD";
        private Guid ID;
        private Guid SoftDeletedID;

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

        private List<Message> getDeletedMessages()
        {
            return
            [
                new (){ Content = CONTENT, CreationDate = DateTime.Now.AddDays(-1), Email = EMAIL, Name = NAME, Username = USERNAME, IsDeleted = true, DeletedOn = DateTime.Now},
                new (){ Content = "2c", CreationDate = DateTime.Now.AddDays(-2), Email = "2e", Name = "2n", Username = "2u", IsDeleted = true, DeletedOn = DateTime.Now},
                new (){ Content = "3c", CreationDate = DateTime.Now.AddDays(-3), Email = "3e", Name = "3n", Username = "3u", IsDeleted = true, DeletedOn = DateTime.Now},
                new (){ Content = "4c", CreationDate = DateTime.Now.AddDays(-4), Email = "4e", Name = "4n", Username = "4u", IsDeleted = true, DeletedOn = DateTime.Now},
                new (){ Content = "5c", CreationDate = DateTime.Now.AddDays(-5), Email = "5e", Name = "5n", Username = "5u", IsDeleted = true, DeletedOn = DateTime.Now}
            ];
        }

        public MessageApiIntegrationTests(CustomWebApplicationFactory factory)
        {
            client = factory.CreateClient();
            using (var scope = factory.Services.CreateScope())
            {
                ApplicationDbContext context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

                context.RemoveRange(context.Messages);
                context.RemoveRange(context.BadWords);
                context.SaveChanges();
                context.RemoveRange(context.Messages);
                context.AddRange(getMessages());
                context.AddRange(getDeletedMessages());
                context.AddRange([new BadWord() { Word = BADWORD}]);
                context.SaveChanges();

                ID = context.Messages.Where(m => m.Username == USERNAME && !m.IsDeleted).FirstOrDefault().Id;
                SoftDeletedID = context.Messages.Where(m => m.Username == USERNAME && m.IsDeleted).FirstOrDefault().Id;
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

            Assert.True(messageList.All(m => m.Username == USERNAME));
        }

        [Fact]
        public async Task GetMessagesFilteringContent()
        {

            var response = await client.GetAsync("/api/messages?SearchInContent=" + CONTENT);
            response.EnsureSuccessStatusCode();
            var result = await response.Content.ReadAsStringAsync();
            var messageList = JsonConvert.DeserializeObject<List<MessageResponseModel>>(result);

            Assert.True(messageList.All(m => m.Content == CONTENT));
        }

        [Fact]
        public async Task GetMessagesFilteringName()
        {

            var response = await client.GetAsync("/api/messages?SearchInName=" + NAME);
            response.EnsureSuccessStatusCode();
            var result = await response.Content.ReadAsStringAsync();
            var messageList = JsonConvert.DeserializeObject<List<MessageResponseModel>>(result);

            Assert.True(messageList.All(m => m.Name == NAME));
        }

        private string? GetToken(string username, bool isAdmin = false, bool isManagement = false)
        {
            List<Claim> authClaims = new List<Claim>() { new Claim(ClaimTypes.Name, username), new Claim(ClaimTypes.Email, EMAIL) };

            if (isAdmin)
            {
                authClaims.Add(new(ClaimTypes.Role, "Admin"));
            }

            if (isManagement)
            {
                authClaims.Add(new(Constants.ServiceClaimType, Constants.ManagementServiceName));
            }

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
        public async Task PostMessageWithoutBadWordsWhenLoggedIn()
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
        public async Task PostMessageWithBadWordsWhenLoggedIn()
        {

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", GetToken(USERNAME));
            var toPost = new AddMessageDto { Content = "content" + BADWORD, Name = "name" };
            var response = await client.PostAsJsonAsync("/api/messages", toPost);

            Assert.Equal(StatusCodes.Status406NotAcceptable, ((int)response.StatusCode));
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
            Assert.Equal(CONTENT, message.Content);
            Assert.Equal(NAME, message.Name);
            Assert.Equal(USERNAME, message.Username);
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

        [Fact]
        public async Task RestoreYourMessageWhenLoggedIn()
        {
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", GetToken(USERNAME));
            var response = await client.PatchAsync("/api/messages/" + SoftDeletedID, null);
            response.EnsureSuccessStatusCode();
        }

        [Fact]
        public async Task RestoreNotYourMessageWhenLoggedIn()
        {
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", GetToken(USERNAME + "notEmpty"));
            var response = await client.PatchAsync("/api/messages/" + SoftDeletedID, null);

            Assert.Equal(StatusCodes.Status401Unauthorized, ((int)response.StatusCode));
        }

        [Fact]
        public async Task RestoreMessageWhenNotLoggedIn()
        {
            var response = await client.PatchAsync("/api/messages/" + SoftDeletedID, null);

            Assert.Equal(StatusCodes.Status401Unauthorized, ((int)response.StatusCode));
        }

        [Fact]
        public async Task GetDeletedMessagesWhenLoggedIn()
        {
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", GetToken(USERNAME));
            var response = await client.GetAsync("/api/messages/GetDeleted");
            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadAsStringAsync();
            var messageList = JsonConvert.DeserializeObject<List<MessageResponseModel>>(result);

            Assert.Equal(getDeletedMessages().Where(m => m.Username == USERNAME).Count(), messageList.Count);
        }

        [Fact]
        public async Task GetDeletedMessagesWhenNotLoggedIn()
        {
            var response = await client.GetAsync("/api/messages/GetDeleted");

            Assert.Equal(StatusCodes.Status401Unauthorized, ((int)response.StatusCode));
        }

        [Fact]
        public async Task AddBadWordWhenNotAdmin()
        {
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", GetToken(USERNAME));
            var response = await client.PostAsJsonAsync("/api/BadWords", new List<string>() { ""});

            Assert.Equal(StatusCodes.Status403Forbidden, ((int)response.StatusCode));
        }

        [Fact]
        public async Task AddBadWordWhenAdmin()
        {
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", GetToken(USERNAME, true));
            var response = await client.PostAsJsonAsync("/api/BadWords", new List<string>() { "" });

            response.EnsureSuccessStatusCode();
        }

        [Fact]
        public async Task RemoveBadWordWhenNotAdmin()
        {
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", GetToken(USERNAME));
            var response = await client.DeleteAsync("/api/BadWords?badWord=" + BADWORD);

            Assert.Equal(StatusCodes.Status403Forbidden, ((int)response.StatusCode));
        }

        [Fact]
        public async Task RemoveExistingBadWordWhenAdmin()
        {
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", GetToken(USERNAME, true));
            var response = await client.DeleteAsync("/api/BadWords?badWord=" + BADWORD);

            response.EnsureSuccessStatusCode();
        }

        [Fact]
        public async Task RemoveMissingBadWordWhenAdmin()
        {
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", GetToken(USERNAME, true));
            var response = await client.DeleteAsync("/api/BadWords?badWord=" + Guid.NewGuid().ToString());

            Assert.Equal(StatusCodes.Status404NotFound, ((int)response.StatusCode));
        }

        [Fact]
        public async Task DeleteUserMessagesWhenAuthorized()
        {
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", GetToken(USERNAME, false, true));
            var response = await client.DeleteAsync("/api/services/DeleteUserMessages/username");

            response.EnsureSuccessStatusCode();
        }

        [Fact]
        public async Task DeleteUserMessagesWhenNotAuthorized()
        {
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", GetToken(USERNAME));
            var response = await client.DeleteAsync("/api/services/DeleteUserMessages/username");

            Assert.Equal(StatusCodes.Status403Forbidden, ((int)response.StatusCode));
        }

        [Fact]
        public async Task DeleteUserMessagesWhenNotAuthenticated()
        {
            var response = await client.DeleteAsync("/api/services/DeleteUserMessages/username");

            Assert.Equal(StatusCodes.Status403Forbidden, ((int)response.StatusCode));
        }

        [Fact]
        public async Task RenameUserMessagesWhenAuthorized()
        {
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", GetToken(USERNAME, false, true));
            var response = await client.PatchAsync($"/api/services/RenameUser?oldUsername={USERNAME}&newUsername=newUsername", null);

            response.EnsureSuccessStatusCode();
        }

        [Fact]
        public async Task RenameUserMessagesWhenNotAuthorized()
        {
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", GetToken(USERNAME));
            var response = await client.PatchAsync("/api/services/RenameUser?oldUsername=oldUsername&newUsername=newUsername", null);

            Assert.Equal(StatusCodes.Status403Forbidden, ((int)response.StatusCode));
        }

        [Fact]
        public async Task RenameUserMessagesWhenNotAuthenticated()
        {
            var response = await client.PatchAsync("/api/services/RenameUser?oldUsername=oldUsername&newUsername=newUsername", null);

            Assert.Equal(StatusCodes.Status403Forbidden, ((int)response.StatusCode));
        }
    }
}
