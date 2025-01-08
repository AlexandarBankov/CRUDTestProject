using CRUDTestProject.Data.Entities;
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
            var Message = dbContext.Messages.Find(id);
            
            if (Message is null)
            {
                throw new ArgumentException();
            }
            
            dbContext.Messages.Remove(Message);
        }

        public IEnumerable<Message> GetAll()
        {
            return dbContext.Messages.Include(m => m.User);
        }

        public Message? GetById(Guid id)
        {
            return dbContext.Messages.Include(m => m.User).ToList().Find(m => m.Id == id);
        }

        public User GetUser()
        {
            return dbContext.Users.First();
        }

        public void Insert(Message message)
        {
            dbContext.Messages.Add(message);
        }

        public void Save()
        {
            dbContext.SaveChanges();
        }

        public void Update(Guid id, string name, string content)
        {
            var message = dbContext.Messages.Find(id);

            if (message is null)
            {
                throw new ArgumentException();
            }

            message.Name = name;
            message.Content = content;
        }
    }
}
