using CRUDTestProject.Data.Entities;

namespace CRUDTestProject.Models.Response
{
    public class MessageResponseModel(Message message)
    {
        public Guid Id { get; set; } = message.Id;
        public string Name { get; set; } = message.Name;
        public string Content { get; set; } = message.Content;

        public DateTime CreationDate { get; set; } = message.CreationDate;

        //Data of the poster
        public string Username { get; set; } = message.Username;
    }
}
