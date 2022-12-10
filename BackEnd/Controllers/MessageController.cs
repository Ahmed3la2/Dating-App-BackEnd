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

        [HttpGet]
        public async Task<ActionResult<IEnumerable<MessageDto>>> GetMessageForUser([FromQuery] MessageParam messageParam)
        {
            var UserName = User.GetUsername();
            messageParam.UserName = UserName;

            var Message = await _messageRepository.GetMessageForUser(messageParam);

            Response.AddPaginationHeader(Message.CurrentPage, Message.PageSize, Message.Count, Message.TotalPages);

            return Ok(Message);
        } 

        [HttpGet("thread/{username}")]

        public async Task<ActionResult<IEnumerable<MessageDto>>> GetConversation(string username)
        {
            var current = User.GetUsername();

            return Ok(await _messageRepository.GetMessageThread(current, username));
        }
    }
}
