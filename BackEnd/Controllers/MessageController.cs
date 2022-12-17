using AutoMapper;
using BackEnd.Data;
using BackEnd.DTOS;
using BackEnd.Entities;
using BackEnd.Extensions;
using BackEnd.Helpers;
using BackEnd.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace BackEnd.Controllers
{
    [ServiceFilter(typeof(LogUserActivity))]
    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous]
    public class MessageController : ControllerBase
    {
        private readonly IMessageRepository _messageRepository;
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;

        public MessageController(IMessageRepository messageRepository, IUserRepository userRepository, IMapper mapper)
        {
            _messageRepository = messageRepository;
            _userRepository = userRepository;
            _mapper = mapper;
        }

        //Send Messages Between Two Users
        [HttpPost]
        public async Task<ActionResult<MessageDto>> CreateMessage(CreateMessageDto createMessageDto)
        {
            var UserName = User.GetUsername();
            if (UserName == createMessageDto.RecipientUsername.ToLower()) return BadRequest("You Cannot Send Message To Your Self");

            var Sender = await _userRepository.GetUserByNameAsync(UserName);
            var reciver = await _userRepository.GetUserByNameAsync(createMessageDto.RecipientUsername);

            if (reciver == null) return NotFound();

            var message = new Message
            {
                Sender = Sender,
                Recipient = reciver,
                Content = createMessageDto.Content,
                SenderUserName = Sender.UserName,
                RecipientUserName = reciver.UserName
            };
            _messageRepository.AddMessage(message);
            if ( await _messageRepository.SaveAllAsync()) return Ok(_mapper.Map<MessageDto>(message));

            return BadRequest("Cannot Create message");
        }

        //Get InBox And OutBox Message For Current User
        [HttpGet]
        public async Task<ActionResult<IEnumerable<MessageDto>>> GetMessageForUser([FromQuery] MessageParam messageParam)
        {
            var UserName = User.GetUsername();
            messageParam.UserName = UserName;

            var Message = await _messageRepository.GetMessageForUser(messageParam);

            Response.AddPaginationHeader(Message.CurrentPage, Message.PageSize, Message.TotalCount, Message.TotalPages);

            return Ok(Message);
        } 

        //Get Conversation Between Two Users
        [HttpGet("thread")]

        public async Task<ActionResult<IEnumerable<MessageDto>>> GetConversation([FromQuery]MessageParam messageParam)
        {
            var current = User.GetUsername();

            var messages = await _messageRepository.GetMessageThread(current, messageParam);

            Response.AddPaginationHeader(messages.CurrentPage, messages.PageSize, messages.TotalCount, messages.TotalPages);

            return Ok(messages);
        }

        //Delete Message By Send It's ID
        [HttpDelete("{id}")]

        public async Task DeleteMessage(int id)
        {
            var message = await _messageRepository.GetMessageAsync(id);
            _messageRepository.DeleteMessage(message);
        }
    }
}
