using MinimalChatApplication.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinimalChatApp.DataAccess.Interface
{
    public interface IMessageRepository
    {
        Task<bool> isExistReceiver(string Receverid);
        Task<Message> FindMessage(string MessageId);

        Task<object> getLogs(DateTime? startTime = null, DateTime? endTime = null);
        Task<bool> isExistGroup(string groupid);



    }
}
