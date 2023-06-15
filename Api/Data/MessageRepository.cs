using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Api.DTOs;
using Api.Helpers;
using Api.Interfaces;
using Api.Models;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;

namespace Api.Data
{
    public class MessageRepository : IMessageRepository
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;
        public MessageRepository(DataContext context, IMapper mapper)
        {
            _mapper = mapper;
            _context = context;
        }

        public void AddMessage(Message message)
        {
            _context.Messages.Add(message);
        }

        public void DeleteMessage(Message message)
        {
            _context.Messages.Remove(message);
        }

        public async Task<Message> GetMessage(int id)
        {
            return await _context.Messages.FindAsync(id);
        }

        public async Task<PagedList<MessageDTO>> GetMessagesForUser(MessageParams messageParams)
        {
            var query = _context.Messages
                .OrderByDescending(m => m.MessageSent)
                .AsQueryable();

            query = messageParams.Container switch
            {
                "Inbox" => query.Where(u => u.Recipient.UserName == messageParams.UserName),
                "Outbox" => query.Where(u => u.Sender.UserName == messageParams.UserName),
                _ => query.Where(u => u.Recipient.UserName == messageParams.UserName && u.DateRead == null)
            };

            var message = query.ProjectTo<MessageDTO>(_mapper.ConfigurationProvider);
            
            return await PagedList<MessageDTO>.CreateAsync(message, messageParams.PageNumber, messageParams.PageSize);

        }
        public async Task<IEnumerable<MessageDTO>> GetMessageThread(string currentUserName, 
             string recipientUserName)
         {
             var messages = await _context.Messages
                 .Include(u => u.Sender).ThenInclude(p => p.Photos)
                 .Include(u => u.Recipient).ThenInclude(p => p.Photos)
                 .Where(m => m.Recipient.UserName == currentUserName && m.RecipientDeleted == false
                         && m.Sender.UserName == recipientUserName
                         || m.Recipient.UserName == recipientUserName
                         && m.Sender.UserName == currentUserName && m.SenderDeleted == false
                 )
                 .OrderBy(m => m.MessageSent)
                 .ToListAsync();

             var unreadMessages = messages.Where(m => m.DateRead == null 
                 && m.Recipient.UserName == currentUserName).ToList();

             if (unreadMessages.Any())
             {
                 foreach (var message in unreadMessages)
                 {
                     message.DateRead = DateTime.Now;
                 }

                 await _context.SaveChangesAsync();
             }

             return _mapper.Map<IEnumerable<MessageDTO>>(messages);
         }

        public async Task<bool> SaveAllAsync()
        {
            return await _context.SaveChangesAsync() > 0;
        }
    }
}