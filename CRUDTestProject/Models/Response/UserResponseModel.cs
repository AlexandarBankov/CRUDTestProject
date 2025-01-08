using CRUDTestProject.Data.Entities;

namespace CRUDTestProject.Models.Response
{
    public class UserResponseModel
    {
        public UserResponseModel(User user)
        {
            this.Id = user.Id;
            this.Username = user.Username;
            this.Email = user.Email;
            this.FirstName = user.FirstName;
            this.LastName = user.FirstName;
        }
        public Guid Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }
}
