using CRUDTestProject.Data.Entities;

namespace CRUDTestProject.Data
{
    public interface IMessageRepository
    {
        IEnumerable<Message> GetAll();

        Message? GetById(Guid id);

        void Insert(Message message);
        void Update(Guid id, string name, string content);
        void Delete(Guid id);
        User GetUser();
        void Save();
    }
}
