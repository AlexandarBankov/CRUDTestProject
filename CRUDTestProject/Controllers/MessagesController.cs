﻿using CRUDTestProject.Data.Entities;
using CRUDTestProject.Models;
using CRUDTestProject.Models.Response;
using CRUDTestProject.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CRUDTestProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MessagesController(IMessageHandler messageHandler) : ControllerBase
    {
        //gets the username from the claims of an authorized user (null for an unauthorized user)
        private string? username => User.FindFirstValue(ClaimTypes.Name);


        [HttpGet]
        public IActionResult GetMessagesPassingFilter(
            [FromQuery] MessageFilterParameters filterParameters,
            [FromQuery] string? matchUsername,
            [FromQuery] bool isOrderAscending = true)
        {
            IEnumerable<Message> result = isOrderAscending ?
                messageHandler.GetAll().OrderBy(m => m.CreationDate)
                : messageHandler.GetAll().OrderByDescending(m => m.CreationDate);

            if (matchUsername is not null)
            {
                result = result.Where(m => m.Username == matchUsername);
            }
            result = filterParameters.FilterMessages(result);
            

            return Ok(
                result
                .Select(m => new MessageResponseModel(m))
                .ToList()
                );
        }

        [HttpGet]
        [Route("{id:guid}")]
        public IActionResult GetMessageById(Guid id)
        {
            var message = messageHandler.GetById(id);

            if (message is null)
            {
                return NotFound();
            }

            return Ok(new MessageResponseModel(message));
        }

        [HttpPost]
        [Authorize]
        public IActionResult AddMessage(AddMessageDto addMessageDto)
        {
            var messageEntity = new Message 
            {
                Name = addMessageDto.Name, 
                Content = addMessageDto.Content,
                CreationDate = DateTime.Now,
                Username = username,
                Email = User.FindFirstValue(ClaimTypes.Email)
            };

            messageHandler.Insert(messageEntity);
            
            return Ok(new MessageResponseModel(messageEntity));
        }

        [HttpPut]
        [Authorize]
        [Route("{id:guid}")]
        public IActionResult UpdateMessage(Guid id, UpdateMessageDto updateMessageDto) 
        {
            Message message = messageHandler.Update(id, updateMessageDto.Name, updateMessageDto.Content, username);
            
            return Ok(new MessageResponseModel(message));
        }

        [HttpDelete]
        [Authorize]
        [Route("{id:guid}")]
        public IActionResult DeleteMessage(Guid id) 
        {
            messageHandler.Delete(id, username);

            return NoContent();
        }

        [HttpPatch]
        [Authorize]
        [Route("{id:guid}")]
        public IActionResult RestoreMessage(Guid id)
        {
            messageHandler.Restore(id, username);

            return Ok();
        }

        [HttpGet]
        [Authorize]
        [Route("[action]")]
        public IActionResult GetDeleted()
        {
            var messages = messageHandler.GetDeleted()
                .Where(m => m.Username == username);

            return Ok(messages.Select(m => new MessageResponseModel(m)));
        }

        [HttpGet]
        [Authorize]
        [Route("[action]")]
        public IActionResult GetUsername()
        {
            var name = username;

            return Ok(name);
        }
    }
}
