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
        Message Update(Guid id, string name, string content);
        void Delete(Guid id);
        void Restore(Guid id);

        string? GetPosterUsernameById(Guid id);
    }
}
