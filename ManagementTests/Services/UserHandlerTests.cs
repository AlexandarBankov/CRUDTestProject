using Management.Data.Entities;
using Management.Middleware.Exceptions;
using Management.Models;
using Management.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using Refit;
using System.Security.Claims;

namespace ManagementTests.Services
{
    public class UserHandlerTests
    {
        const string USERNAME = "username";
        const string DELETABLE_USERNAME = "DELETABLE_USERNAME";
        const string PASSWORD = "password";
        private readonly string JWTSECRET = Guid.NewGuid().ToString() + Guid.NewGuid().ToString();
        
        private User deletableUser = new User() { TenantId = "", Email = "", UserName = DELETABLE_USERNAME, Id = "" };
        
        private readonly IUserHandler userHandler;
        private Mock<UserManager<User>> mockManager;
        private Mock<IMessagesApi> mockApi;
        

        public UserHandlerTests()
        {
            mockApi = new();
            mockManager = new(Mock.Of<IUserStore<User>>(), null, null, null, null, null, null, null, null);
            var user = new User() { TenantId = "", Email = "", UserName = USERNAME };
            

            mockManager.Setup(m => m.FindByNameAsync(USERNAME)).ReturnsAsync(user);
            mockManager.Setup(m => m.FindByNameAsync(DELETABLE_USERNAME)).ReturnsAsync(deletableUser);
            mockManager.Setup(m => m.IsInRoleAsync(user, "Admin")).ReturnsAsync(true);
            mockManager.Setup(m => m.IsInRoleAsync(deletableUser, "Admin")).ReturnsAsync(false);
            mockManager.Setup(m => m.CheckPasswordAsync(user, PASSWORD)).ReturnsAsync(true);
            mockManager.Setup(m => m.GetRolesAsync(user)).ReturnsAsync(["Admin"]);

            mockManager.Setup(m => m.CreateAsync(It.IsAny<User>(), It.IsAny<string>())).ReturnsAsync(IdentityResult.Success);
            
            Mock<IConfiguration> mockConfig = new();
            mockConfig.Setup(c => c["JWT:Secret"]).Returns(JWTSECRET);
            mockConfig.Setup(c => c["JWT:Issuer"]).Returns("");
            mockConfig.Setup(c => c["JWT:Audience"]).Returns("");

            ApiResponse<object> failedResponse = new(new HttpResponseMessage(System.Net.HttpStatusCode.NotFound), null, null);
            ApiResponse<object> succeededResponse = new(new HttpResponseMessage(System.Net.HttpStatusCode.OK), null, null);

            mockApi.Setup(m => m.DeleteUserMessages(DELETABLE_USERNAME)).ReturnsAsync(failedResponse);
            mockApi.Setup(m => m.RenameUser(DELETABLE_USERNAME, It.IsAny<string>())).ReturnsAsync(succeededResponse);

            userHandler = new UserHandler(mockManager.Object, mockConfig.Object, mockApi.Object, Mock.Of<ILogger<UserHandler>>());
        }

        [Fact]
        public async Task AuthenticateWithCorrectCredentials()
        {
            var token = await userHandler.Authenticate(new LoginUserModel() { Password = PASSWORD, Username = USERNAME });

            Assert.Equal(USERNAME, token.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name).Value);
            Assert.Contains(token.Claims, c => c.Type == ClaimTypes.Role && c.Value == "Admin");
        }

        [Fact]
        public async Task AuthenticateWithIncorrectUsername()
        {
            await Assert.ThrowsAsync<NoSuchUserException>(() => userHandler.Authenticate(new LoginUserModel() { Password = PASSWORD, Username = Guid.NewGuid().ToString() }));
        }

        [Fact]
        public async Task AuthenticateWithIncorrectPassword()
        {
            await Assert.ThrowsAsync<NoSuchUserException>(() => userHandler.Authenticate(new LoginUserModel() { Password = Guid.NewGuid().ToString(), Username = USERNAME }));
        }

        [Fact]
        public async Task CreateUserWithExistingUsername()
        {
            await Assert.ThrowsAsync<UserAlreadyExistsException>(() => userHandler.Create(new RegisterUserModel() { Username = USERNAME , Password = PASSWORD, Email = "", TenantId = ""}));
        }

        [Fact]
        public async Task CreateUserWithNewUsername()
        {
            var toCreate = new RegisterUserModel() { Username = USERNAME + "notEmpty", Password = PASSWORD, Email = "", TenantId = "" };
            await userHandler.Create(toCreate);

            mockManager.Verify(m => m.CreateAsync(It.IsAny<User>(), toCreate.Password), Times.Once);
        }

        [Fact]
        public async Task BanUserWithMissingUsername()
        {
            await Assert.ThrowsAsync<NoSuchUserException>(() => userHandler.Ban(USERNAME + "notEmpty", false));
        }

        [Fact]
        public async Task BanUserWhoHasAdminRole()
        {
            await Assert.ThrowsAsync<UnauthorizedException>(() => userHandler.Ban(USERNAME, false));
        }

        [Fact]
        public async Task BanUserApiCallFails()
        {
            await Assert.ThrowsAsync<ApiCallFailedException>(() => userHandler.Ban(DELETABLE_USERNAME, true));
        }

        [Fact]
        public async Task BanUserApiCallSucceeds()
        {
            await userHandler.Ban(DELETABLE_USERNAME, false);

            mockManager.Verify(m => m.DeleteAsync(deletableUser), Times.Once);
        }
    }
}
