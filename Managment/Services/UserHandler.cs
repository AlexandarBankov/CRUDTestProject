using Management.Data.Entities;
using Management.Middleware.Exceptions;
using Management.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Query.Internal;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Management.Services
{
    public class UserHandler(UserManager<User> userManager,
        IConfiguration configuration) : IUserHandler
    {
        public async Task<JwtSecurityToken> Authenticate(LoginUserModel model)
        {
            var user = await userManager.FindByNameAsync(model.Username);
            if (user is null || !await userManager.CheckPasswordAsync(user, model.Password))
            {
                throw new NoSuchUserException("Incorrect username or password.");
            }

            var authClaims = new List<Claim>
                {
                    new(ClaimTypes.Name, user.UserName),
                    new(ClaimTypes.Email, user.Email),
                    new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                };

            return GetToken(authClaims);
        }

        public async Task Create(RegisterUserModel model)
        {
            var userExists = await userManager.FindByNameAsync(model.Username);
            if (userExists is not null)
                throw new UserAlreadyExistsException("User already exists!");

            User user = new()
            {
                Email = model.Email,
                SecurityStamp = Guid.NewGuid().ToString(),
                UserName = model.Username,
                TenantId = model.TenantId
            };
            var result = await userManager.CreateAsync(user, model.Password);

            if (!result.Succeeded)
                throw new UserCreationException("User creation failed! Please check user details and try again.");
        }

        private JwtSecurityToken GetToken(List<Claim> authClaims)
        {
            var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JWT:Secret"]));

            var token = new JwtSecurityToken(
                issuer: configuration["JWT:Issuer"],
                audience: configuration["JWT:Audience"],
                expires: DateTime.Now.AddHours(1),
                claims: authClaims,
                signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
                );

            return token;
        }
    }
}
