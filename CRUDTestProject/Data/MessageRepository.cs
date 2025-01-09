using CRUDTestProject.Data.Entities;
using CRUDTestProject.Middleware.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace CRUDTestProject.Data
{
    public class MessageRepository : IMessageRepository
    {
        private readonly ApplicationDbContext dbContext;
        public MessageRepository(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }
        public void Delete(Guid id)
        {
            var message = dbContext.Messages.Find(id);
            
            if (message is null)
            {
                throw new NotFoundException("Message with given id wasn't found so it cannot be deleted.");
            }
            
            dbContext.Messages.Remove(message);
            dbContext.SaveChanges();
        }

        public IEnumerable<Message> GetAll()
        {
            return dbContext.Messages.Include(m => m.User).AsNoTracking();
        }

        public Message? GetById(Guid id)
        {
            return dbContext.Messages.Include(m => m.User).Where(m => m.Id == id).AsNoTracking().FirstOrDefault();
        }

        public User GetUser()
        {
            return dbContext.Users.First();
        }

        public void Insert(Message message)
        {
            dbContext.Messages.Add(message);
            dbContext.SaveChanges();
        }

        public Message Update(Guid id, string name, string content)
        {
            var message = dbContext.Messages.Include(m => m.User).Where(m => m.Id == id).FirstOrDefault();

            if (message is null)
            {
                throw new NotFoundException("Message with given id wasn't found so it cannot be updated.");
            }

            message.Name = name;
            message.Content = content;
            
            dbContext.SaveChanges();

            return message;
        }
    }
}
