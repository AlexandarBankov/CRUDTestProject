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

        public void Delete(Guid id)
        {
            var message = notDeleted.Where(m => m.Id == id).FirstOrDefault() ?? throw new NotFoundException("Invalid message id for deletion.");
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

        public string? GetPosterUsernameById(Guid id)
        {
            var message = repository.GetById(id);

            if (message is not null)
            {
                return message.Username;
            }

            return null;
        }

        public void Insert(Message message)
        {
            repository.Insert(message);
        }

        public void Restore(Guid id)
        {
            var message = deleted.Where(m => m.Id == id).FirstOrDefault() ?? throw new NotFoundException("Invalid message id for restoration.");

            repository.Restore(message);
        }

        public Message Update(Guid id, string name, string content)
        {
            var message = notDeleted.Where(m => m.Id == id).FirstOrDefault() ?? throw new NotFoundException("Invalid message id for update.");

            repository.Update(message, name, content);

            return message;
        }
    }
}
