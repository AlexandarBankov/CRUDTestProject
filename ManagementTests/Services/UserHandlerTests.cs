using Management.Data.Entities;
using Management.Middleware.Exceptions;
using Management.Models;
using Management.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Moq;
using System.Security.Claims;

namespace ManagementTests.Services
{
    public class UserHandlerTests
    {
        const string USERNAME = "username";
        const string PASSWORD = "password";
        private readonly string JWTSECRET = Guid.NewGuid().ToString() + Guid.NewGuid().ToString();

        private readonly IUserHandler userHandler;
        private Mock<UserManager<User>> mockManager;
        

        public UserHandlerTests()
        {
            mockManager = new(Mock.Of<IUserStore<User>>(), null, null, null, null, null, null, null, null);
            var user = new User() { TenantId = "", Email = "", UserName = USERNAME };
            mockManager.Setup(m => m.FindByNameAsync(USERNAME)).ReturnsAsync(user);
            mockManager.Setup(m => m.CheckPasswordAsync(user, PASSWORD)).ReturnsAsync(true);
            mockManager.Setup(m => m.GetRolesAsync(user)).ReturnsAsync([]);

            mockManager.Setup(m => m.CreateAsync(It.IsAny<User>(), It.IsAny<string>())).ReturnsAsync(IdentityResult.Success);
            
            Mock<IConfiguration> mockConfig = new();
            mockConfig.Setup(c => c["JWT:Secret"]).Returns(JWTSECRET);
            mockConfig.Setup(c => c["JWT:Issuer"]).Returns("");
            mockConfig.Setup(c => c["JWT:Audience"]).Returns("");

            userHandler = new UserHandler(mockManager.Object, mockConfig.Object);
        }

        [Fact]
        public async Task AuthenticateWithCorrectCredentials()
        {
            var token = await userHandler.Authenticate(new LoginUserModel() { Password = PASSWORD, Username = USERNAME });

            Assert.Equal(USERNAME, token.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name).Value);
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
    }
}
