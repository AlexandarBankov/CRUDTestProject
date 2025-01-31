using Management.Data.Entities;
using Management.Middleware.Exceptions;
using Management.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using Refit;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Management.Services
{
    public class UserHandler(UserManager<User> userManager,
        IConfiguration configuration,
        IMessagesApi messagesApi,
        ILogger<UserHandler> logger) : IUserHandler
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
            var roles = await userManager.GetRolesAsync(user);
            foreach (var role in roles)
            {
                authClaims.Add(new(ClaimTypes.Role, role));
            }

            return GetToken(authClaims);
        }

        public async Task Ban(string username, bool removeMessages)
        {
            var user = await userManager.FindByNameAsync(username) ?? throw new NoSuchUserException($"User with username '{username}' wasn't found.");

            if (await userManager.IsInRoleAsync(user, "Admin")) throw new UnauthorizedException($"{username} is an admin, they cannot be banned.");

            ApiResponse<object>? response;

            if (removeMessages)
            {
                response = await messagesApi.DeleteUserMessages(username);
            }
            else
            {
                string newName = $"[Banned]_({user.Id})";

                response = await messagesApi.RenameUser(username, newName);
            }

            if (!response.IsSuccessStatusCode)
            {
                var message = $"The request about user '{username}' to the messages api failed.";
                logger.LogError(message);
                throw new ApiCallFailedException(message);
            }

            await userManager.DeleteAsync(user);
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
