using CRUDTestProject.Data.Entities;

namespace CRUDTestProject.Services
{
    public interface IMessageHandler
    {
        //returns all messages that haven't been deleted
        IEnumerable<Message> GetAll();
        IEnumerable<Message> GetDeleted();

        Message? GetById(Guid id);

        void Insert(Message message);
        Message Update(Guid id, string name, string content, string username);
        void Delete(Guid id, string username);
        void Restore(Guid id, string username);
    }
}
