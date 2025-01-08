using CRUDTestProject.Data;
using CRUDTestProject.Data.Entities;
using CRUDTestProject.Models;
using CRUDTestProject.Models.Response;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CRUDTestProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MessagesController : ControllerBase
    {
        private readonly IMessageRepository messageRepository;

        public MessagesController(IMessageRepository messageRepository)
        {
            this.messageRepository = messageRepository;
        }


        [HttpGet]
        public IActionResult getAllMessagesContaining(
            [FromQuery] string? searchInName,
            [FromQuery] string? searchInContent,
            [FromQuery] string? matchUsername,
            [FromQuery] DateTime? messageIsAfter,
            [FromQuery] DateTime? messageIsBefore,
            [FromQuery] bool isOrderAscending = true)
        {
            IEnumerable<Message>? result = isOrderAscending ?
                messageRepository.GetAll().OrderBy(m => m.CreationDate)
                : messageRepository.GetAll().OrderByDescending(m => m.CreationDate);

            if (searchInContent is not null)
            {
                result = result.Where(m => m.Content.Contains(searchInContent));
            }
            if (searchInName is not null)
            {
                result = result.Where(m => m.Name.Contains(searchInName));
            }
            if (matchUsername is not null)
            {
                result = result.Where(m => m.User.Username == matchUsername);
            }
            if (messageIsAfter is not null)
            {
                result = result.Where(m => m.CreationDate > messageIsAfter);
            }
            if (messageIsBefore is not null)
            {
                result = result.Where(m => m.CreationDate < messageIsBefore);
            }

            return Ok(
                result
                .Select(m => new MessageResponseModel(m))
                .ToList()
                );
        }

        [HttpGet]
        [Route("{id:guid}")]
        public IActionResult getMessageById(Guid id)
        {
            var message = messageRepository.GetById(id);

            if (message is null)
            {
                return NotFound();
            }

            return Ok(new MessageResponseModel(message));
        }

        [HttpPost]
        public IActionResult AddMessage(AddMessageDto addMessageDto)
        {
            var messageEntity = new Message 
            {
                Name = addMessageDto.Name, 
                Content = addMessageDto.Content,
                CreationDate = DateTime.Now,
                User = messageRepository.GetUser()
            };

            messageRepository.Insert(messageEntity);
            
            return Ok(new MessageResponseModel(messageEntity));
        }

        [HttpPut]
        [Route("{id:guid}")]
        public IActionResult UpdateMessage(Guid id, UpdateMessageDto updateMessageDto) 
        {
            Message message;
            try
            {
                message = messageRepository.Update(id, updateMessageDto.Name, updateMessageDto.Content);
            }
            catch (ArgumentNullException)
            {
                return NotFound();
            }

            return Ok(new MessageResponseModel(message));
        }

        [HttpDelete]
        [Route("{id:guid}")]
        public IActionResult DeleteMessage(Guid id) 
        {
            try
            {
                messageRepository.Delete(id);
            }
            catch (ArgumentNullException)
            {
                return NotFound();
            }

            return Ok();
        }
    }
}
