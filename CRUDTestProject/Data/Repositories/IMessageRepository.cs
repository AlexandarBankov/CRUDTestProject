using CRUDTestProject.Data.Entities;

namespace CRUDTestProject.Data.Repositories
{
    public interface IMessageRepository
    {
        IQueryable<Message> Messages { get; }
        //Not to be tracked for changes
        Message? GetById(Guid id);
        void Delete(Message message);
        void Insert(Message message);
        void Restore(Message message);
        void Update(Message message, string name, string content);
    }
}
