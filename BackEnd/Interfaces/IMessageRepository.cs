using BackEnd.DTOS;
using BackEnd.Entities;
using BackEnd.Helpers;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BackEnd.Interfaces
{
    public interface IMessageRepository
    {
        void AddMessage(Message messageRepository);

        void DeleteMessage(Message messageRepository);

        Task<Message> GetMessageAsync(int MessageId);

        Task<PageList<MessageDto>> GetMessageForUser(MessageParam messageParam);

        Task<PageList<MessageDto>> GetMessageThread(string CurrentUserName, MessageParam messageParam);

        Task<bool> SaveAllAsync();
    }
}
