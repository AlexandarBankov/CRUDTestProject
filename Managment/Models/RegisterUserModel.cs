
using SharedLibrary;
using System.ComponentModel.DataAnnotations;

namespace Management.Models
{
    public class RegisterUserModel
    {
        [MaxLength(Constants.UsernameMaxLength)]
        public required string Username { get; set; }
        public required string Password { get; set; }
        [MaxLength(Constants.EmailMaxLength)]
        public required string Email { get; set; }
        public required string TenantId { get; set; }
    }
}
