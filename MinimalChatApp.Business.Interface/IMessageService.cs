using MinimalChatApplication.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinimalChatApp.Business.Interface
{
    public interface IMessageService
    {
        Task<Message> sendMessage(MessageRequest request, string user);
        Task<Message> EditMessage(EditMessageRequest request, Message message);
        Task DeleteMessage(Message message);
        Task<List<Message>> GetConversationHistory(ConversationHistoryRequest request, string userId);
    }
}
