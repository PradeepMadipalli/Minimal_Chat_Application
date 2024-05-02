using MinimalChatApp.Model;
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
        Task<Insertmessage> sendMessage(ChatMessageRequest request);
        Task<List<GetGroups>> GetGetGroups();
        string GetNameIdentifier();
        Task<Group> CreateGroup(string groupname, string userlist);
        Task<UserStatuss> UpdateUserStatus(string userId, int status);
        Task<object> UpdateGroupUsers(string groupId, string userslist);
        Task<List<UserGroup>> GetUserGroup(string UserId);
        Task<int> GetUserStatus(string userId);


    }
}
