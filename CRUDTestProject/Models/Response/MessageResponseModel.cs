using CRUDTestProject.Data.Entities;

namespace CRUDTestProject.Models.Response
{
    public class MessageResponseModel
    {
        public MessageResponseModel(Message message)
        {
            this.Id = message.Id;
            this.Name = message.Name;
            this.Content = message.Content;
            this.CreationDate = message.CreationDate;
            this.User = new UserResponseModel(message.User);
        }
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Content { get; set; }

        public DateTime CreationDate { get; set; }

        public UserResponseModel User { get; set; }
    }
}
