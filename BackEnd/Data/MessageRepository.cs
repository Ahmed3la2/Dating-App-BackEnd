using BackEnd.DTOS;
using BackEnd.Entities;
using BackEnd.Helpers;
using BackEnd.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BackEnd.Data
{
    public class MessageRepository : IMessageRepository
    {
        private readonly DataContext _context;

        public MessageRepository(DataContext context)
        {
            _context = context;
        }

        public void AddMessage(Message message)
        {
            _context.messages.Add(message);

        }

        public void DeleteMessage(Message message)
        {
           _context.messages.Remove(message);
        }

        public async Task<Message> GetMessageAsync(int MessageId)
        {
            return await _context.messages.FindAsync(MessageId); 
        }


        //Get Messages That Users Send And Recive
        public async Task<PageList<MessageDto>> GetMessageForUser(MessageParam  messageParam)
        {
            var query = _context.messages.OrderByDescending(m => m.MessageSent).AsQueryable();

            query = messageParam.Container switch
            {
                "inbox" => query.Where(m => m.Recipient.UserName == messageParam.UserName),
                "outbox" => query.Where(m => m.Sender.UserName == messageParam.UserName),
                _ => query.Where(m => m.Recipient.UserName == messageParam.UserName && m.DateRead == null)
            };

            var message = query.Select(q => new MessageDto
            {
                Id = q.Id,
                Content = q.Content,
                DateRead = q.DateRead,
                MessageSent = q.MessageSent,
                RecipientId = q.RecipientId,
                RecipientPhotoUrl = q.Recipient.Photos.FirstOrDefault(p => p.IsMain == true).Url,
                RecipientUserName= q.Recipient.UserName,
                SenderId= q.SenderId,
                SenderPhtoUrl= q.Sender.Photos.FirstOrDefault(p => p.IsMain == true).Url,
                SenderUserName=q.SenderUserName,
                
            });

            return await PageList<MessageDto>.CreateAsync(message, messageParam.PageSize, messageParam.PageNumber);
        }

        //Get Conversation Between Two Users
        public async Task<IEnumerable<MessageDto>> GetMessageThread(string CurrentUserName, string RecipientName)
        {
           var messages = _context.messages
                                  .Include(m => m.Sender).ThenInclude(s => s.Photos)
                                  .Include(m => m.Recipient).ThenInclude(s => s.Photos)
                                  .OrderByDescending(m => m.MessageSent)
                                  .Where(m => m.SenderUserName == CurrentUserName 
                                   && m.RecipientUserName == RecipientName
                                   || m.SenderUserName == RecipientName
                                   && m.RecipientUserName == CurrentUserName).AsQueryable();


            var unreadMessages = messages.Where(m => m.DateRead == null && m.RecipientUserName == CurrentUserName);

            if (unreadMessages.Any())
            {
                foreach (var unreadMessage in unreadMessages)
                {
                    unreadMessage.DateRead = DateTime.Now;
                }
                await _context.SaveChangesAsync();
            }


            var message = messages.Select(m => new MessageDto
            {
                Content= m.Content,
                RecipientUserName = m.Recipient.UserName,
                SenderUserName = m.SenderUserName,
                RecipientPhotoUrl= m.Recipient.Photos.FirstOrDefault(p => p.IsMain == true).Url,
                SenderPhtoUrl= m.Sender.Photos.FirstOrDefault(p => p.IsMain == true).Url,
                DateRead= m.DateRead,
                Id=m.Id,
                MessageSent=m.MessageSent,
                RecipientId=m.Recipient.Id,
            });

            return await message.ToListAsync();
        }

        //Save ConText
        public async Task<bool> SaveAllAsync()
        {
           return await _context.SaveChangesAsync() > 0;
        }
    }
}
