using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using MinimalChatApp.Business.Interface;
using MinimalChatApp.Common;
using MinimalChatApp.DataAccess.Interface;
using MinimalChatApp.Model;
using MinimalChatApplication.Model;
using System.Security.Claims;

namespace MinimialChatApp.Api.Controllers
{
    [Route("api/")]
    [ApiController]

    [EnableCors("AllowOrigin")]
    public class MessageController : ControllerBase
    {
        private readonly ChatDBContext _context;
        private readonly IMessageRepository _messageRepository;
        private readonly IMessageService _messageService;
        private readonly ChatHub _chatHub;

        public MessageController(ChatDBContext context, IMessageRepository messageRepository, IMessageService messageService, ChatHub chatHub)
        {
            _context = context;
            _messageService = messageService;
            _chatHub = chatHub;
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
                //var messages =  _chatHub.sendMessage(request.Content);
                return Ok(message);
            }
            else
            {
                return BadRequest("Message sending failed due to validation errors");
            }
        }

        [HttpPost]
        [Route("EditMessage")]
        public async Task<IActionResult> EditMessage(EditMessageRequest request)
        {

            var senderId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            Message message = await _messageRepository.FindMessage(request.Messageid);
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
        [Route("Conversation")]
        public async Task<IActionResult> GetConversationHistory(ConversationHistoryRequest request)
        {

            int userstatus = 0;
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
                return NotFound(new { errormessages = "Conversation not found." });
            }

            return Ok(new { messages});
        }
        [HttpGet]
        [Route("GetCurrentstatus")]
        public async Task<IActionResult> Getstatus()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            int status = await _messageService.GetUserStatus(userId);
            return Ok(status);
        }

    }
}
