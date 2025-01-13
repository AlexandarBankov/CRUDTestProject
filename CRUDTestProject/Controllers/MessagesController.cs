using CRUDTestProject.Data;
using CRUDTestProject.Data.Entities;
using CRUDTestProject.Models;
using CRUDTestProject.Models.Response;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

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
        public IActionResult getMessagesPassingFilter(
            [FromQuery] MessageFilterParameters filterParameters,
            [FromQuery] string? matchUsername,
            [FromQuery] bool isOrderAscending = true)
        {
            IEnumerable<Message> result = isOrderAscending ?
                messageRepository.GetAll().OrderBy(m => m.CreationDate)
                : messageRepository.GetAll().OrderByDescending(m => m.CreationDate);

            if (matchUsername is not null)
            {
                result = result.Where(m => m.User.Username == matchUsername);
            }
            result = filterParameters.filterMessages(result);
            

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
            Message message = messageRepository.Update(id, updateMessageDto.Name, updateMessageDto.Content);
            
            return Ok(new MessageResponseModel(message));
        }

        [HttpDelete]
        [Route("{id:guid}")]
        public IActionResult DeleteMessage(Guid id) 
        {
            messageRepository.Delete(id);

            return Ok();
        }

        [HttpGet]
        [Authorize]
        [Route("[action]")]
        public IActionResult getUsername()
        {
            var name = User.FindFirstValue(ClaimTypes.Name);

            return Ok(name);
        }
    }
}
