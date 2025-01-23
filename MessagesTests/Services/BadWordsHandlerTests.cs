using CRUDTestProject.Data.Entities;
using CRUDTestProject.Data.Repositories;
using CRUDTestProject.Middleware.Exceptions;
using CRUDTestProject.Services;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessagesTests.Services
{
    public class BadWordsHandlerTests
    {
        private IBadWordsHandler handler;
        private Mock<IBadWordsRepository> mockRepository;
        IEnumerable<BadWord> badWords;
        const string BADWORD = "BADDDDDDDDDDDDDDDDDD";
        private readonly string MISSING = Guid.NewGuid().ToString();
        public BadWordsHandlerTests()
        {
            badWords = [new() { Word = BADWORD}];
            mockRepository = new();

            mockRepository.Setup(m => m.BadWords).Returns(badWords);

            handler = new BadWordsHandler(mockRepository.Object);
        }

        [Fact]
        public void CheckForBadWordsShouldThrowOnBadWord() 
        {
            Assert.Throws<BadWordException>(() => handler.CheckForBadWords([BADWORD]));
        }

        [Fact]
        public void RemoveMissing()
        {
            Assert.Throws<NotFoundException>(() => handler.Remove(MISSING));
        }

        [Fact]
        public void RemoveExisting()
        {
            handler.Remove(BADWORD);

            mockRepository.Verify(m => m.Remove(It.IsAny<BadWord>()), Times.Once);
        }
    }
}
