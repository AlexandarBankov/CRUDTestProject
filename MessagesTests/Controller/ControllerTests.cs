using CRUDTestProject.Controllers;
using CRUDTestProject.Data.Entities;
using CRUDTestProject.Models.Response;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Security.Claims;

namespace MessagesTests.Controller
{
    public class ControllerTests(RepositoryFixture fixture) : IClassFixture<RepositoryFixture>
    {
        private readonly MessagesController controller = new(fixture.Repository);

        [Fact]
        public void GetByIdOfExistingMessage()
        {
            var response = controller.GetMessageById(fixture.ExistingId);

            Assert.IsType<OkObjectResult>(response);

            var result = response as OkObjectResult;

            Assert.Equal(result.StatusCode, StatusCodes.Status200OK);
            Assert.IsType<MessageResponseModel>(result.Value);

            var message = result.Value as MessageResponseModel;

            Assert.Equal(message.Name, fixture.message.Name);
            Assert.Equal(message.Content, fixture.message.Content);
            Assert.Equal(message.Username, fixture.message.Username);
            Assert.Equal(message.Id, fixture.message.Id);
            Assert.Equal(message.CreationDate, fixture.message.CreationDate);
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
            var user = new ClaimsPrincipal(new ClaimsIdentity(
            [
                new Claim(ClaimTypes.Name, "username"),
                new Claim(ClaimTypes.Email, "email"),
            ], "mock"));
            controller.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext() { User = user }
            };
            var response = controller.AddMessage(new() { Content = "string", Name = "string" });

            fixture.Mock.Verify(m => m.Insert(It.IsAny<Message>()), Times.Once());
        }
    }
}