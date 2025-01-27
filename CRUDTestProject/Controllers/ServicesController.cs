using CRUDTestProject.Services;
using CRUDTestProject.Services.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CRUDTestProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ServicesController(IBulkMessagesHandler handler) : ControllerBase
    {
        [HttpPatch]
        [ServiceAuthorization("Management")]
        [Route("[action]")]
        public IActionResult RenameUser(string oldUsername, string newUsername)
        {
            handler.RenameUser(oldUsername, newUsername);
            return Ok();
        }

        [HttpDelete]
        [ServiceAuthorization("Management")]
        [Route("[action]/{username}")]
        public IActionResult DeleteUserMessages(string username)
        {
            handler.DeleteUserMessages(username);
            return Ok();
        }
    }
}
