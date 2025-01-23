using CRUDTestProject.Data;
using CRUDTestProject.Data.Entities;
using CRUDTestProject.Middleware.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace CRUDTestProject.Services
{
    public class MessageHandler(IMessageRepository repository) : IMessageHandler
    {
        private IQueryable<Message> notDeleted => repository.Messages.Where(m => !m.IsDeleted);
        private IQueryable<Message> deleted => repository.Messages.Where(m => m.IsDeleted);

        public void Delete(Guid id, string username)
        {
            var message = notDeleted.Where(m => m.Id == id).FirstOrDefault() ?? throw new NotFoundException("Message for deletion not found.");
            
            if (message.Username != username) throw new DifferentUserException("You can delete only your own messages.");
            
            repository.Delete(message);
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

        public void Insert(Message message)
        {
            repository.Insert(message);
        }

        public void Restore(Guid id, string username)
        {
            var message = deleted.Where(m => m.Id == id).FirstOrDefault() ?? throw new NotFoundException("Message for restoration not found.");

            if (message.Username != username) throw new DifferentUserException("You can restore only your own messages.");

            repository.Restore(message);
        }

        public Message Update(Guid id, string name, string content, string username)
        {
            var message = notDeleted.Where(m => m.Id == id).FirstOrDefault() ?? throw new NotFoundException("Message for update not found.");

            if (message.Username != username) throw new DifferentUserException("You can update only your own messages.");

            repository.Update(message, name, content);

            return message;
        }
    }
}
