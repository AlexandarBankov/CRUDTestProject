using CRUDTestProject.Controllers;
using CRUDTestProject.Data.Entities;
using CRUDTestProject.Models.Response;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Security.Claims;

namespace MessagesTests.Controller
{
    public class ControllerTests
    {
        private readonly RepositoryFixture fixture;
        private readonly MessagesController controller;

        public ControllerTests()
        {
            fixture = new();
            controller = new(fixture.Repository);
        }

        private void mockUserInController(string username, string email)
        {
            var user = new ClaimsPrincipal(new ClaimsIdentity(
            [
                new Claim(ClaimTypes.Name, username),
                new Claim(ClaimTypes.Email, email),
            ], "mock"));
            controller.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext() { User = user }
            };
        }

        [Fact]
        public void GetByIdOfExistingMessage()
        {
            var response = controller.GetMessageById(fixture.ExistingId);

            Assert.IsType<OkObjectResult>(response);

            var result = response as OkObjectResult;

            Assert.Equal(StatusCodes.Status200OK, result.StatusCode);
            Assert.IsType<MessageResponseModel>(result.Value);

            var message = result.Value as MessageResponseModel;

            Assert.Equal(fixture.message.Name, message.Name);
            Assert.Equal(fixture.message.Content, message.Content);
            Assert.Equal(fixture.message.Username, message.Username);
            Assert.Equal(fixture.message.Id, message.Id);
            Assert.Equal(fixture.message.CreationDate, message.CreationDate);
        }

        [Fact]
        public void GetByIdOfMissingMessage()
        {
            var response = controller.GetMessageById(fixture.MissingId);

            Assert.IsType<NotFoundResult>(response);
        }

        [Fact]
        public void AddMessage()
        {
            const string USERNAME = "username";
            const string EMAIL = "email";
            const string CONTENT = "content";
            const string NAME = "name";

            mockUserInController(USERNAME, EMAIL);

            var response = controller.AddMessage(new() { Content = CONTENT, Name = NAME });

            fixture.Mock.Verify(m => m.Insert(It.IsAny<Message>()), Times.Once());

            Assert.IsType<OkObjectResult>(response);

            var result = response as OkObjectResult;
            Assert.Equal(StatusCodes.Status200OK, result.StatusCode);
            Assert.IsType<MessageResponseModel>(result.Value);

            var message = result.Value as MessageResponseModel;

            Assert.Equal(NAME, message.Name);
            Assert.Equal(CONTENT, message.Content);
            Assert.Equal(USERNAME, message.Username);
        }

        [Fact]
        public void DeleteMessageOfSomeoneElse()
        {
            mockUserInController(fixture.message.Username + "notEmpty", string.Empty);

            var response = controller.DeleteMessage(fixture.message.Id);

            Assert.IsType<UnauthorizedObjectResult>(response);

            fixture.Mock.Verify(m => m.Delete(It.IsAny<Guid>()), Times.Never());
        }

        [Fact]
        public void DeleteYourMessage()
        {
            mockUserInController(fixture.message.Username, string.Empty);

            var response = controller.DeleteMessage(fixture.message.Id);

            Assert.IsType<OkResult>(response);

            fixture.Mock.Verify(m => m.Delete(It.IsAny<Guid>()), Times.Once());
        }
    }
}