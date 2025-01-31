using CRUDTestProject.Services;
using CRUDTestProject.Services.Authorization;
using Microsoft.AspNetCore.Mvc;
using SharedLibrary;

namespace CRUDTestProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ServicesController(IBulkMessagesHandler handler) : ControllerBase
    {
        [HttpPatch]
        [ServiceAuthorization(Constants.ManagementServiceName)]
        [Route("[action]")]
        public IActionResult RenameUser(string oldUsername, string newUsername)
        {
            handler.RenameUser(oldUsername, newUsername);
            return Ok();
        }

        [HttpDelete]
        [ServiceAuthorization(Constants.ManagementServiceName)]
        [Route("[action]/{username}")]
        public IActionResult DeleteUserMessages(string username)
        {
            handler.DeleteUserMessages(username);
            return NoContent();
        }
    }
}
