using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CRUDTestProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles ="Admin")]
    public class BackupsController : ControllerBase
    {
        [HttpPost]
        [Route("[action]")]
        public IActionResult Create()
        {

            return Ok();
        }

        [HttpGet]
        [Route("[action]")]
        public IActionResult Get()
        {

            return Ok();
        }
    }
}
