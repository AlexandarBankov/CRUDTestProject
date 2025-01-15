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
    }
}
