using CRUDTestProject.Data.Entities;
using CRUDTestProject.Middleware.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace CRUDTestProject.Data
{
    public class MessageRepository(ApplicationDbContext dbContext) : IMessageRepository
    {
        private IQueryable<Message> notDeleted => dbContext.Messages.Where(m => !m.IsDeleted);
        private IQueryable<Message> deleted => dbContext.Messages.Where(m => m.IsDeleted);

        public void Delete(Guid id)
        {
            var message = notDeleted.Where(m => m.Id == id).FirstOrDefault() ?? throw new NotFoundException("Invalid message id for deletion.");
            dbContext.Messages.Remove(message);
            dbContext.SaveChanges();
        }

        public IEnumerable<Message> GetAll()
        {
            return notDeleted.AsNoTracking();
        }

        public Message? GetById(Guid id)
        {
            return notDeleted.Where(m => m.Id == id).AsNoTracking().FirstOrDefault();
        }

        public IEnumerable<Message> GetDeleted()
        {
            return deleted.AsNoTracking();
        }

        public string? GetPosterUsernameById(Guid id)
        {
            var message = dbContext.Messages.Find(id);

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

        public void Restore(Guid id)
        {
            var message = deleted.Where(m => m.Id == id).FirstOrDefault() ?? throw new NotFoundException("Invalid message id for restoration.");
            message.IsDeleted = false;
            message.DeletedOn = null;
            dbContext.SaveChanges();
        }

        public Message Update(Guid id, string name, string content)
        {
            var message = notDeleted.Where(m => m.Id == id).FirstOrDefault() ?? throw new NotFoundException("Invalid message id for update.");
            message.Name = name;
            message.Content = content;
            
            dbContext.SaveChanges();

            return message;
        }
    }
}
