using CRUDTestProject.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections;

namespace CRUDTestProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BadWordsController(IBadWordsHandler handler) : ControllerBase
    {
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public IActionResult AddBadWords(IEnumerable<string> badWords)
        {
            handler.AddRange(badWords);
            return Ok();
        }

        [HttpDelete]
        [Authorize(Roles = "Admin")]
        public IActionResult RemoveBadWord(string badWord)
        {
            handler.Remove(badWord);
            return Ok();
        }
    }
}
