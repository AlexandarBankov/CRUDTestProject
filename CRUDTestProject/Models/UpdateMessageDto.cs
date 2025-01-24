using SharedLibrary;
using System.ComponentModel.DataAnnotations;

namespace CRUDTestProject.Models
{
    public class UpdateMessageDto
    {
        [MaxLength(Constants.MessageNameMaxLength)]
        public required string Name { get; set; }

        [MaxLength(Constants.MessageContentMaxLength)]
        public required string Content { get; set; }
    }
}
