using CRUDTestProject.Data.Entities;

namespace CRUDTestProject.Data
{
    public interface IMessageRepository
    {
        IEnumerable<Message> GetAll();

        Message? GetById(Guid id);

        void Insert(Message message);
        Message Update(Guid id, string name, string content);
        void Delete(Guid id);
    }
}
