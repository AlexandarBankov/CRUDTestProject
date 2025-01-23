using CRUDTestProject.Data.Entities;
using CRUDTestProject.Data.Repositories;
using CRUDTestProject.Middleware.Exceptions;
using CRUDTestProject.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace MessagesTests.Repository
{
    public class HandlerTests
    {
        private IMessageHandler handler;
        Mock<IMessageRepository> mockRepository;

        private List<Message> messages = [new() { Content = "Content", CreationDate = DateTime.Now, Email = "Email", Name = "Name", Username = "Username", Id = Guid.NewGuid() },
                                          new() { Content = "Content", CreationDate = DateTime.Now, Email = "Email", Name = "Name", Username = "Username", Id = Guid.NewGuid() , IsDeleted = true}];
        private readonly Guid missingId = Guid.NewGuid();
        public HandlerTests()
        {
            mockRepository = new();

            mockRepository.Setup(m => m.Messages).Returns(messages.AsQueryable());
            mockRepository.Setup(m => m.GetById(messages[0].Id)).Returns(messages[0]);

            handler = new MessageHandler(mockRepository.Object, Mock.Of<IBadWordsHandler>());
        }

        [Fact]
        public void DeleteExistingWithSameUsername()
        {
            handler.Delete(messages[0].Id, messages[0].Username);

            mockRepository.Verify(m => m.Delete(messages[0]), Times.Once);
        }

        [Fact]
        public void DeleteExistingWithDifferentUsername()
        {
            Assert.Throws<DifferentUserException>(() => handler.Delete(messages[0].Id, messages[0].Username + "notEmpty"));

            mockRepository.Verify(m => m.Delete(messages[0]), Times.Never);
        }

        [Fact]
        public void DeleteMissing()
        {
            Assert.Throws<NotFoundException>(() => handler.Delete(missingId, string.Empty));
        }

        [Fact]
        public void RestoreExistingWithSameUsername()
        {
            handler.Restore(messages[1].Id, messages[1].Username);

            mockRepository.Verify(m => m.Restore(messages[1]), Times.Once);
        }

        [Fact]
        public void RestoreExistingWithDifferentUsername()
        {
            Assert.Throws<DifferentUserException>(() => handler.Restore(messages[1].Id, messages[1].Username + "notEmpty"));

            mockRepository.Verify(m => m.Restore(messages[1]), Times.Never);
        }

        [Fact]
        public void RestoreMissing()
        {
            Assert.Throws<NotFoundException>(() => handler.Restore(missingId, string.Empty));
        }

        [Fact]
        public void UpdateExistingWithSameUsername()
        {
            const string updatedContent = "upContent";
            const string updatedName = "upName";
            var result = handler.Update(messages[0].Id, updatedName, updatedContent, messages[0].Username);

            mockRepository.Verify(m => m.Update(messages[0], updatedName, updatedContent), Times.Once);
        }

        [Fact]
        public void UpdateExistingWithDifferentUsername()
        {
            const string updatedContent = "upContent";
            const string updatedName = "upName";
            Assert.Throws<DifferentUserException>(() => handler.Update(messages[0].Id, updatedName, updatedContent, messages[0].Username + "notEmpty"));

            mockRepository.Verify(m => m.Update(messages[0], updatedName, updatedContent), Times.Never);
        }

        [Fact]
        public void UpdateMissing()
        {
            const string updatedContent = "upContent";
            const string updatedName = "upName";

            Assert.Throws<NotFoundException>(() => handler.Update(missingId, updatedName, updatedContent, string.Empty));
        }
    }
}
