using Management.Models;
using System.IdentityModel.Tokens.Jwt;

namespace Management.Services
{
    public interface IUserHandler
    {
        public Task Create(RegisterUserModel model);
        public Task<JwtSecurityToken> Authenticate(LoginUserModel model);
    }
}
