﻿
using MinimalChatApp.Business.Interface;
using MinimalChatApp.DataAccess.Interface;
using MinimalChatApplication.Model;
using MinimalChatApp.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace MinimalChatApp.Business
{
    public class MessageService : IMessageService
    {
        private readonly IMessageRepository _messagerepository;
        private readonly ChatDBContext _chatDBContext;

        public MessageService(IMessageRepository messageRepository, ChatDBContext chatDBContext)
        {
            _messagerepository = messageRepository;
            _chatDBContext = chatDBContext;
        }

        public async Task DeleteMessage(Message message)
        {
            _chatDBContext.Messages.Remove(message);
            await _chatDBContext.SaveChangesAsync();
        }

        public async Task<Message> EditMessage(EditMessageRequest request, Message message)
        {
            message.Content = request.Content;

            await _chatDBContext.SaveChangesAsync();


            return message;
        }

        public async Task<List<Message>> GetConversationHistory(ConversationHistoryRequest request, string userId)
        {
            DateTime beforeDate = request.Before ?? DateTime.UtcNow;
            var sortOrder = request.Sort == "desc" ? SortOrder.Descending : SortOrder.Ascending;

            var messagesQuery = _chatDBContext.Messages
                   .Where(m => (m.SenderId == userId && m.ReceiverId == request.UserId) || (m.SenderId == request.UserId && m.ReceiverId == userId))
                   .Where(m => m.Timestamp < beforeDate)
                   .OrderBy(m => sortOrder == SortOrder.Ascending ? m.Timestamp : (DateTime?)null)
                   .OrderByDescending(m => sortOrder == SortOrder.Descending ? m.Timestamp : (DateTime?)null)
                   .Take(request.Count);


           List<Message> messages = await messagesQuery.Select(m =>  new Message
            {
                MessageId = m.MessageId,
                SenderId = m.SenderId,
                ReceiverId = m.ReceiverId,
                Content = m.Content,
                Timestamp = m.Timestamp
            }).ToListAsync();
            return messages;
        }



        public async Task<Message> sendMessage(MessageRequest request, string user)
        {
            var message = new Message
            {
                MessageId = Guid.NewGuid(),
                SenderId = user,
                ReceiverId = request.ReceiverId,
                Content = request.Content,
                Timestamp = DateTime.UtcNow
            };

            _chatDBContext.Messages.Add(message);
            await _chatDBContext.SaveChangesAsync();
            return message;
        }
    }
}
