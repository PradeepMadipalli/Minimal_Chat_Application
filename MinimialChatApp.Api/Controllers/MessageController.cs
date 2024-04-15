﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using MinimalChatApp.Business.Interface;
using MinimalChatApp.DataAccess.Interface;
using MinimalChatApplication.Model;
using System.Security.Claims;

namespace MinimialChatApp.Api.Controllers
{
    [Route("api/")]
    [ApiController]
    public class MessageController : ControllerBase
    {
        private readonly ChatDBContext _context;
        private readonly IMessageRepository _messageRepository;
        private readonly IMessageService _messageService;

        public MessageController(ChatDBContext context, IMessageRepository messageRepository, IMessageService messageService)
        {
            _context = context;
            _messageService = messageService;
            _messageRepository = messageRepository;
        }
        [HttpPost]
        [Route("send")]
        public async Task<IActionResult> SendMessage(MessageRequest request)
        {
            if (ModelState.IsValid)
            {
                var user = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var receiverExists = await _messageRepository.isExistReceiver(request.ReceiverId);
                if (!receiverExists)
                {
                    return BadRequest(new { error = "Receiver does not exist." });
                }
                Message message = await _messageService.sendMessage(request, user);
                return Ok(message);
            }
            else
            {
                return BadRequest("Message sending failed due to validation errors");
            }
        }

        [HttpPut("{messageId}")]
        public async Task<IActionResult> EditMessage(string messageId, EditMessageRequest request)
        {

            var senderId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            Message message = await _messageRepository.FindMessage(messageId);
            if (message == null)
            {
                return NotFound(new { error = "Message not found." });
            }
            if (message.SenderId != senderId)
            {
                return Unauthorized(new { error = "You are not authorized to edit this message." });
            }
            try
            {
                Message message1 = await _messageService.EditMessage(request, message);
                return Ok(message1);
            }
            catch (Exception)
            {
                return BadRequest(new { error = "Failed to edit the message." });
            }
        }

        [HttpDelete("{messageId}")]
        public async Task<IActionResult> DeleteMessage(string messageId)
        {
            var senderId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            Message message = await _messageRepository.FindMessage(messageId);
            if (message == null)
            {
                return NotFound(new { error = "Message not found." });
            }
            if (message.SenderId != senderId)
            {
                return Unauthorized(new { error = "You are not authorized to delete this message." });
            }
            await _messageService.DeleteMessage(message);

            return Ok(new { message = "Message deleted successfully." });
        }
        [HttpPost]
        [Route("GetConversationHistory")]
        public async Task<IActionResult> GetConversationHistory(ConversationHistoryRequest request)
        {

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;


            if (request.Count <= 0 || request.Count > 100)
            {
                return BadRequest(new { error = "Invalid count parameter. Count should be between 1 and 100." });
            }

            DateTime beforeDate = request.Before ?? DateTime.UtcNow;
            var sortOrder = request.Sort == "desc" ? SortOrder.Descending : SortOrder.Ascending;

            var messages = await _messageService.GetConversationHistory(request, userId);

            if (messages.Count == 0)
            {
                return NotFound(new { error = "Conversation not found." });
            }

            return Ok(new { messages });
        }
    }
}