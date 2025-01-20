using CRUDTestProject.Data;
using CRUDTestProject.Data.Entities;
using CRUDTestProject.Middleware.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace MessagesTests.Repository
{
    public class RepositoryTests
    {
        private IMessageRepository repository;
        private DbContextOptions<ApplicationDbContext> options;
        private ApplicationDbContext context; 
        private readonly Message message = new() { Content = "Content", CreationDate = DateTime.Now, Email = "Email", Name = "Name", Username = "Username", Id = Guid.NewGuid() };
        private readonly Guid missingId = Guid.NewGuid();
        public RepositoryTests()
        {
            options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: "database")
            .Options;

            // Insert seed data into the database using one instance of the context
            context = new ApplicationDbContext(options);

            context.RemoveRange(context.Messages);

            context.Messages.Add(message);
            context.SaveChanges();

            repository = new MessageRepository(context);
        }

        [Fact]
        public void DeleteExisting() 
        {
            repository.Delete(message.Id);

            Assert.Null(context.Messages.Find(message.Id));
        }

        [Fact]
        public void DeleteMissing()
        {
            Assert.Throws<NotFoundException>(() => repository.Delete(missingId));
        }

        [Fact]
        public void UpdateExisting()
        {
            const string updatedContent = "upContent";
            const string updatedName = "upName";
            var result = repository.Update(message.Id, updatedName, updatedContent);

            Assert.Equal(updatedName, result.Name);
            Assert.Equal(updatedContent, result.Content);
            Assert.Equal(updatedName, context.Messages.Find(result.Id).Name);
            Assert.Equal(updatedContent, context.Messages.Find(result.Id).Content);
        }

        [Fact]
        public void UpdateMissing()
        {
            const string updatedContent = "upContent";
            const string updatedName = "upName";
            
            Assert.Throws<NotFoundException>(() => repository.Update(missingId, updatedName, updatedContent));
        }

        [Fact]
        public void GetPosterUsernameByExistingId()
        {
            var result = repository.GetPosterUsernameById(message.Id);

            Assert.Equal(message.Username, result);
        }

        [Fact]
        public void GetPosterUsernameByMissingId()
        {
            var result = repository.GetPosterUsernameById(missingId);

            Assert.Null(result);
        }
    }
}
