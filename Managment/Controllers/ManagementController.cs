using Management.Data.Entities;
using Management.Models;
using Management.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
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
    }
}
