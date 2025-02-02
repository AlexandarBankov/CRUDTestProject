using Management.Models;
using Management.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Management.Controllers
{
    [ApiController]
    [Route("/auth")]
    public class UsersController(IUserHandler userHandler) : ControllerBase
    {
        [HttpPost]
        [Route("user")]
        public async Task<IActionResult> LoginAsync([FromBody] LoginUserModel model)
        {
            var token = await userHandler.Authenticate(model);
            return Ok(new JwtSecurityTokenHandler().WriteToken(token));
        }

        [HttpGet]
        [Authorize]
        public IActionResult Get()
        {

            return Ok(User.FindFirstValue(ClaimTypes.Name));
        }

    }
}
