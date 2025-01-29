using CRUDTestProject.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CRUDTestProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles ="Admin")]
    public class BackupsController(IBackupHandler handler) : ControllerBase
    {
        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> Create()
        {
            await handler.Create();
            return Ok();
        }

        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> Get()
        {
            var results = await handler.GetNames();
            return Ok(results);
        }
    }
}
