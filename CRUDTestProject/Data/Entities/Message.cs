using SharedLibrary;
using System.ComponentModel.DataAnnotations;

namespace CRUDTestProject.Data.Entities
{
    public class Message : ISoftDeletable
    {
        public Guid Id { get; set; }

        [MaxLength(Constants.MessageNameMaxLength)]
        public required string Name { get; set; }

        [MaxLength(Constants.MessageContentMaxLength)]
        public required string Content { get; set; }
        public required DateTime CreationDate { get; set; }

        //Data of the poster
        [MaxLength(Constants.UsernameMaxLength)]
        public required string Username { get; set; }

        [MaxLength(Constants.EmailMaxLength)]
        public required string Email { get; set; }


        public bool IsDeleted { get; set; }
        public DateTime? DeletedOn { get; set; }
    }
}
