using CRUDTestProject.Data.Entities;
using System.Text.Json.Serialization;

namespace CRUDTestProject.Models.Response
{
    public class MessageResponseModel
    {
        public MessageResponseModel()
        {
            
        }
        public MessageResponseModel(Message message)
        {
            this.Id = message.Id;
            this.Name = message.Name;
            this.Content = message.Content;
            this.CreationDate = message.CreationDate;
            this.Username = message.Username;
        }
        public Guid Id { get; set; } 
        public string Name { get; set; } 
        public string Content { get; set; } 

        public DateTime CreationDate { get; set; } 

        //Data of the poster
        public string Username { get; set; } 
    }
}
