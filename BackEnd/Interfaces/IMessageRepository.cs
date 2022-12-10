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

        Task<IEnumerable<MessageDto>> GetMessageThread(string CurrentUserName, string RecipientName);

        Task<bool> SaveAllAsync();
    }
}
