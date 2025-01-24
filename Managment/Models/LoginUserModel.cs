using SharedLibrary;
using System.ComponentModel.DataAnnotations;

namespace Management.Models
{
    public class LoginUserModel
    {
        [MaxLength(Constants.UsernameMaxLength)]
        public required string Username { get; set; }
        public required string Password { get; set; }

    }
}
