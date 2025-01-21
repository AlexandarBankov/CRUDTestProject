using CRUDTestProject.Data;
using CRUDTestProject.Data.Entities;
using CRUDTestProject.Middleware.Exceptions;
using CRUDTestProject.Services;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace MessagesTests.Repository
{
    public class HandlerTests
    {
        private IMessageHandler handler;
        Mock<IMessageRepository> mockRepository;

        private List<Message> messages = [new() { Content = "Content", CreationDate = DateTime.Now, Email = "Email", Name = "Name", Username = "Username", Id = Guid.NewGuid() }];
        private readonly Guid missingId = Guid.NewGuid();
        public HandlerTests()
        {
            mockRepository = new();
            
            mockRepository.Setup(m => m.Messages).Returns(messages.AsQueryable());
            mockRepository.Setup(m => m.GetById(messages[0].Id)).Returns(messages[0]);

            handler = new MessageHandler(mockRepository.Object);
        }

        [Fact]
        public void DeleteExisting() 
        {
            handler.Delete(messages[0].Id);

            mockRepository.Verify(m => m.Delete(messages[0]), Times.Once);
        }

        [Fact]
        public void DeleteMissing()
        {
            Assert.Throws<NotFoundException>(() => handler.Delete(missingId));
        }

        [Fact]
        public void UpdateExisting()
        {
            const string updatedContent = "upContent";
            const string updatedName = "upName";
            var result = handler.Update(messages[0].Id, updatedName, updatedContent);

            mockRepository.Verify(m => m.Update(messages[0], updatedName, updatedContent), Times.Once);
        }

        [Fact]
        public void UpdateMissing()
        {
            const string updatedContent = "upContent";
            const string updatedName = "upName";
            
            Assert.Throws<NotFoundException>(() => handler.Update(missingId, updatedName, updatedContent));
        }

        [Fact]
        public void GetPosterUsernameByExistingId()
        {
            var result = handler.GetPosterUsernameById(messages[0].Id);

            Assert.Equal(messages[0].Username, result);
        }

        [Fact]
        public void GetPosterUsernameByMissingId()
        {
            var result = handler.GetPosterUsernameById(missingId);

            Assert.Null(result);
        }
    }
}
