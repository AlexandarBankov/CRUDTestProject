using CRUDTestProject.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CRUDTestProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class AdminController(IBadWordsHandler badWordsHandler, IBackupHandler backupHandler) : ControllerBase
    {
        [HttpPost]
        [Route("[action]")]
        public IActionResult AddBadWords(IEnumerable<string> badWords)
        {
            badWordsHandler.AddRange(badWords);
            return Ok();
        }

        [HttpDelete]
        [Route("[action]")]
        public IActionResult RemoveBadWord(string badWord)
        {
            badWordsHandler.Remove(badWord);
            return NoContent();
        }

        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> CreateBackup()
        {
            await backupHandler.Create();
            return Ok();
        }

        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> GetBackups()
        {
            var results = await backupHandler.GetNames();
            return Ok(results);
        }
    }
}
