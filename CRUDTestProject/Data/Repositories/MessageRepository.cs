using CRUDTestProject.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace CRUDTestProject.Data.Repositories
{
    public class MessageRepository(ApplicationDbContext dbContext) : IMessageRepository
    {
        public IQueryable<Message> Messages => dbContext.Messages;

        public void Delete(Message message)
        {
            dbContext.Messages.Remove(message);
            dbContext.SaveChanges();
        }

        public Message? GetById(Guid id)
        {
            return Messages.Where(m => m.Id == id).AsNoTracking().FirstOrDefault();
        }

        public void Insert(Message message)
        {
            dbContext.Messages.Add(message);
            dbContext.SaveChanges();
        }

        public void Restore(Message message)
        {
            message.IsDeleted = false;
            message.DeletedOn = null;
            dbContext.SaveChanges();
        }

        public void Update(Message message, string name, string content)
        {
            message.Name = name;
            message.Content = content;

            dbContext.SaveChanges();
        }
    }
}
