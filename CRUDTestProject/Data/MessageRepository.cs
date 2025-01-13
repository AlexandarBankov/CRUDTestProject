using CRUDTestProject.Data.Entities;
using CRUDTestProject.Middleware.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace CRUDTestProject.Data
{
    public class MessageRepository(ApplicationDbContext dbContext) : IMessageRepository
    {
        public void Delete(Guid id)
        {
            var message = dbContext.Messages.Find(id) ?? throw new NotFoundException("Message with given id wasn't found.");
            dbContext.Messages.Remove(message);
            dbContext.SaveChanges();
        }

        public IEnumerable<Message> GetAll()
        {
            return dbContext.Messages.AsNoTracking();
        }

        public Message? GetById(Guid id)
        {
            return dbContext.Messages.Where(m => m.Id == id).AsNoTracking().FirstOrDefault();
        }

        public string? GetPosterUsernameById(Guid id)
        {
            var message = this.GetById(id);

            if (message is not null)
            {
                return message.Username;
            }

            return null;
        }

        public void Insert(Message message)
        {
            dbContext.Messages.Add(message);
            dbContext.SaveChanges();
        }

        public Message Update(Guid id, string name, string content)
        {
            var message = dbContext.Messages.Where(m => m.Id == id).FirstOrDefault() ?? throw new NotFoundException("Message with given id wasn't found so it cannot be updated.");
            message.Name = name;
            message.Content = content;
            
            dbContext.SaveChanges();

            return message;
        }
    }
}
