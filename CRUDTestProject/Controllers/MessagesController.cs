using CRUDTestProject.Data;
using CRUDTestProject.Data.Entities;
using CRUDTestProject.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CRUDTestProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MessagesController : ControllerBase
    {
        private readonly ApplicationDbContext dbContext;

        public MessagesController(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }


        [HttpGet]
        public IActionResult getAllMessagesContaining([FromQuery] string searchString) 
        {
            var filteredMessages = dbContext.Messages.Where(m => m.Content.Contains(searchString)).ToList();

            return Ok(filteredMessages);
        }

        [HttpGet]
        [Route("{id:guid}")]
        public IActionResult getMessageById(Guid id)
        {
            var message = dbContext.Messages.Find(id);

            if (message is null)
            {
                return NotFound();
            }

            return Ok(message);
        }

        [HttpPost]
        public IActionResult AddMessage(AddMessageDto addMessageDto)
        {
            var messageEntity = new Message 
            {
                Name = addMessageDto.Name, 
                Content = addMessageDto.Content
            };

            dbContext.Messages.Add(messageEntity);
            dbContext.SaveChanges();
            
            return Ok(messageEntity);
        }

        [HttpPut]
        [Route("{id:guid}")]
        public IActionResult UpdateMessage(Guid id, UpdateMessageDto updateMessageDto) 
        {
            var message = dbContext.Messages.Find(id);

            if (message is null)
            {
                return NotFound();
            }

            message.Name = updateMessageDto.Name;
            message.Content = updateMessageDto.Content;

            dbContext.SaveChanges();

            return Ok(message);
        }

        [HttpDelete]
        [Route("{id:guid}")]
        public IActionResult DeleteMessage(Guid id) 
        {
            var message = dbContext.Messages.Find(id);

            if (message is null)
            {
                return NotFound();
            }

            dbContext.Messages.Remove(message);
            dbContext.SaveChanges();

            return Ok();
        }
    }
}
