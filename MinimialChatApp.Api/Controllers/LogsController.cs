using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MinimalChatApp.DataAccess.Interface;
using MinimalChatApplication.Model;

namespace MinimialChatApp.Api.Controllers
{
    [Route("api/")]
    [ApiController]
    public class LogsController : ControllerBase
    {
        private readonly IMessageRepository _messsageRepository;
        private readonly ChatDBContext _chatDBContext;

        public LogsController(ChatDBContext chatDBContext, IMessageRepository messageRepository)
        {
            _messsageRepository = messageRepository;
            _chatDBContext = chatDBContext;
        }
        [HttpGet]
        [Route("Logs")]
        public async Task<IActionResult> GetLogs(DateTime? startTime = null, DateTime? endTime = null)
        {
            if (startTime == null)
                startTime = DateTime.Now.AddMinutes(-5);

            if (endTime == null)
                endTime = DateTime.Now;

            var logs = await _chatDBContext.Logs
                .Where(l => l.CreatedDate >= startTime && l.CreatedDate <= endTime)
                .ToListAsync();

            if (logs == null)
                return NotFound(new { error = "No logs found" });

            return Ok(new { Logs = logs });
        }
    }
}
