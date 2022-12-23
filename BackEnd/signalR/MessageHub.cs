using AutoMapper;
using BackEnd.DTOS;
using BackEnd.Entities;
using BackEnd.Extensions;
using BackEnd.Helpers;
using BackEnd.Interfaces;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Threading.Tasks;

namespace BackEnd.signalR
{
    public class MessageHub : Hub
    {
        public readonly IMessageRepository _messageRepository;
        public readonly IMapper _mapper;
        private readonly IUserRepository _userRepository;

        public MessageHub(IMessageRepository messageRepository, IMapper mapper, IUserRepository userRepository )
        {
            _messageRepository = messageRepository;
            _mapper = mapper;
            _userRepository = userRepository;
        }


        public override async Task OnConnectedAsync()
        {
           var HttpContext = Context.GetHttpContext();
           var caller = HttpContext.User.GetUsername();
           var other = HttpContext.Request.Query["user"].ToString();
           var pageNum = Int32.Parse(HttpContext.Request.Query["pageNumber"].ToString());

            var groupName = GetGroupName(caller, other);
           await Groups.AddToGroupAsync(Context.ConnectionId,groupName);

           var messageParam = new MessageParam();
           messageParam.UserName = other;
            messageParam.PageNumber = pageNum;

            var mess = await _messageRepository.GetMessageThread(caller, messageParam);

            var SentMessages = new
            {
                mess,
                mess.TotalCount,
             
            };

            await Clients.Groups(groupName).SendAsync("ReciveMessageThread", SentMessages) ;
        }

        public async Task getPaginatedMessage(int pagenum)
        {
            var HttpContext = Context.GetHttpContext();
            var caller = HttpContext.User.GetUsername();
            var other = HttpContext.Request.Query["user"].ToString();
 

            var groupName = GetGroupName(caller, other);
            await Groups.AddToGroupAsync(Context.ConnectionId, groupName);

            var messageParam = new MessageParam();
            messageParam.UserName = other;
            messageParam.PageNumber = pagenum;

            var mess = await _messageRepository.GetMessageThread(caller, messageParam);

            var SentMessages = new
            {
                mess,
                mess.TotalCount,

            };

            await Clients.Groups(groupName).SendAsync("paginatedMessage", SentMessages);
        }


        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            await base.OnConnectedAsync();
        }

        public async Task SendMessage(CreateMessageDto createMessageDto)
        {
            var UserName = Context.User.GetUsername();
            if (UserName == createMessageDto.RecipientUsername.ToLower()) throw new HubException("You Cannot Send Message To Your Self");

            var Sender = await _userRepository.GetUserByNameAsync(UserName);
            var reciver = await _userRepository.GetUserByNameAsync(createMessageDto.RecipientUsername);

            if (reciver == null) throw new HubException("not-found");

            var message = new Message
            {
                Sender = Sender,
                Recipient = reciver,
                Content = createMessageDto.Content,
                SenderUserName = Sender.UserName,
                RecipientUserName = reciver.UserName
            };
            _messageRepository.AddMessage(message);
            if (await _messageRepository.SaveAllAsync()) 
            {
                var groupName = GetGroupName(Sender.UserName, reciver.UserName);
                await Clients.Group(groupName).SendAsync("NewMessage", _mapper.Map<MessageDto>(message));
            } 

        }

        public static string GetGroupName(string caller, string other)
        {
            var stringComp = string.CompareOrdinal(caller, other) < 0;
            return  stringComp ? $"{caller}-{other}" : $"{other}-{caller}";
        }
    }
}
