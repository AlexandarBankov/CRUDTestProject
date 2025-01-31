using Management.Models;
using Management.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Management.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class ManagementController(IUserHandler userHandler) : ControllerBase
    {
        [HttpPost]
        [Route("user/create")]
        public async Task<IActionResult> RegisterAsync([FromBody] RegisterUserModel model)
        {
            await userHandler.Create(model);

            return Ok();
        }

        [HttpDelete]
        [Authorize(Roles = "Admin")]
        [Route("user/ban/{username}")]
        public async Task<IActionResult> BanUserAsync(string username, bool removeMessages = false)
        {
            await userHandler.Ban(username, removeMessages);

            return NoContent();
        }
    }
}
