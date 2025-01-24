using CRUDTestProject.Data;
using CRUDTestProject.Data.Entities;
using CRUDTestProject.Data.Repositories;
using CRUDTestProject.Middleware.Exceptions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace MessagesTests.Repository
{
    public class MessageRepositoryTests
    {
        private IMessageRepository repository;
        private DbContextOptions<ApplicationDbContext> options;
        private ApplicationDbContext context; 
        private Message message = new() { Content = "Content", CreationDate = DateTime.Now, Email = "Email", Name = "Name", Username = "Username", Id = Guid.NewGuid() };
        private Message toRestore = new() { Content = "Content", CreationDate = DateTime.Now.AddDays(-5), Email = "Email", Name = "Name", Username = "Username", Id = Guid.NewGuid() , DeletedOn = DateTime.Now, IsDeleted = true};
        private readonly Guid missingId = Guid.NewGuid();
        public MessageRepositoryTests()
        {

            options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: "database")
            .AddInterceptors(new SoftDeleteInterceptor())
            .Options;

            // Insert seed data into the database using one instance of the context
            context = new ApplicationDbContext(options);

            context.RemoveRange(context.Messages);

            context.Messages.Add(message);
            context.Messages.Add(toRestore);
            context.SaveChanges();

            repository = new MessageRepository(context);
        }

        [Fact]
        public void DeleteExisting() 
        {
            repository.Delete(message);

            Assert.True(context.Messages.Find(message.Id).IsDeleted);
            Assert.NotNull(context.Messages.Find(message.Id).DeletedOn);
        }

        [Fact]
        public void UpdateExisting()
        {
            const string updatedContent = "upContent";
            const string updatedName = "upName";
            repository.Update(message, updatedName, updatedContent);

            Assert.Equal(updatedName, message.Name);
            Assert.Equal(updatedContent, message.Content);
            Assert.Equal(updatedName, context.Messages.Find(message.Id).Name);
            Assert.Equal(updatedContent, context.Messages.Find(message.Id).Content);
        }

        [Fact]
        public void Insert()
        {
            Message message = new() { Content = "content", Name = "name", CreationDate = DateTime.Now, Email = "Email", Id = Guid.NewGuid(), Username = "Username" };
            repository.Insert(message);

            Assert.True(context.Messages.Contains(message));
        }

        [Fact]
        public void Restore()
        {
            repository.Restore(toRestore);

            Assert.False(toRestore.IsDeleted);
            Assert.Null(toRestore.DeletedOn);
        }

        [Fact]
        public void GetByIdOfExisting()
        {
            var result = repository.GetById(message.Id);

            Assert.NotNull(result);
        }

        [Fact]
        public void GetByIdOfMissing()
        {
            var result = repository.GetById(missingId);

            Assert.Null(result);
        }
    }
}
