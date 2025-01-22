using CRUDTestProject.Controllers;
using CRUDTestProject.Data.Entities;
using CRUDTestProject.Models;
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
        private const string USERNAME = "username";
        private const string EMAIL = "email";
        private const string CONTENT = "content";
        private const string NAME = "name";
        public ControllerTests()
        {
            fixture = new();
            controller = new(fixture.Mock.Object);
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
            mockUserInController(USERNAME, EMAIL);

            var response = controller.AddMessage(new() { Content = CONTENT, Name = NAME });

            fixture.Mock.Verify(m => m.Insert(It.IsAny<Message>()), Times.Once);

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
        public void DeleteYourMessage()
        {
            mockUserInController(fixture.message.Username, string.Empty);

            var response = controller.DeleteMessage(fixture.message.Id);

            Assert.IsType<OkResult>(response);

            fixture.Mock.Verify(m => m.Delete(It.IsAny<Guid>(), fixture.message.Username), Times.Once);
        }

        [Fact]
        public void UpdateYourMessage()
        {
            mockUserInController(fixture.message.Username, string.Empty);

            var response = controller.UpdateMessage(fixture.message.Id, new UpdateMessageDto() { Content = CONTENT, Name = NAME });

            Assert.IsType<OkObjectResult>(response);

            fixture.Mock.Verify(m => m.Update(It.IsAny<Guid>(), NAME, CONTENT, fixture.message.Username), Times.Once);

            var result = response as OkObjectResult;
            Assert.Equal(StatusCodes.Status200OK, result.StatusCode);
            Assert.IsType<MessageResponseModel>(result.Value);
        }

        [Fact]
        public void GetUsernameReturnsUsername()
        {
            mockUserInController(USERNAME, EMAIL);

            var response = controller.GetUsername();

            Assert.IsType<OkObjectResult>(response);

            var result = response as OkObjectResult;
            Assert.Equal(StatusCodes.Status200OK, result.StatusCode);
            Assert.IsType<string>(result.Value);

            var username = result.Value as string;

            Assert.Equal(USERNAME, username);
        }
    }
}