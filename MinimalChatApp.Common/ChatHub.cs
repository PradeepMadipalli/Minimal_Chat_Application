using Microsoft.AspNetCore.SignalR;
using MinimalChatApp.Business.Interface;
using MinimalChatApp.DataAccess.Interface;
using MinimalChatApp.Model;
using MinimalChatApplication.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace MinimalChatApp.Common
{

    public sealed class ChatHub : Hub
    {
        private readonly ChatDBContext _chatDBContext;
        private readonly IMessageService _messageService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        private readonly IMessageRepository _messageRepository;


        public ChatHub(ChatDBContext chatDBContext, IMessageRepository messageRepository,
            IMessageService messageService, IHttpContextAccessor httpContextAccessor)
        {
            _chatDBContext = chatDBContext;
            _messageService = messageService;
            _httpContextAccessor = httpContextAccessor;
            _messageRepository = messageRepository;

        }
        public override async Task OnConnectedAsync()
        {
            await base.OnConnectedAsync();
            var userNameClaim = _httpContextAccessor.HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name);
            var user = userNameClaim?.Value;
            if (user != null)
            {
                var userRooms = _chatDBContext.UserGroups.Where(ur => ur.UserId == user).ToList();
                foreach (var userRoom in userRooms)
                {
                    await Groups.AddToGroupAsync(user, userRoom.GroupId.ToString());
                }
            }
        }
        public async Task sendMessages(string messagess)
        {
            try
            {

                ChatMessageRequest request = JsonConvert.DeserializeObject<ChatMessageRequest>(messagess);
                var receiverExists = await _messageRepository.isExistReceiver(request.ReceiverId);
                var groupexist = await _messageRepository.isExistGroup(request.groupId);
                if (receiverExists || groupexist)
                {

                    Insertmessage message = await _messageService.sendMessage(request);
                    string resmessage = JsonConvert.SerializeObject(message);
                    await Clients.All.SendAsync("receiveMessage", resmessage);
                }
            }
            catch (Exception ex)
            {
                throw new Exception();
            }

        }
        public async Task CreateGroup(string groupName, string userlist)
        {
            try
            {
                Group group = await _messageService.CreateGroup(groupName, userlist);
                var stringv = new { groupId = group.GroupId, groupName = group.GroupName };
                string groupstring = JsonConvert.SerializeObject(stringv);
                await Clients.All.SendAsync("GroupCreated", groupstring);
            }
            catch (Exception ex)
            {
                throw new Exception();
            }

        }

        public async Task EditGroupName(int groupId, string newName)
        {
            var group = await _chatDBContext.Group.FindAsync(groupId);
            if (group != null)
            {
                group.GroupName = newName;
                await _chatDBContext.SaveChangesAsync();
                await Clients.Group(groupId.ToString()).SendAsync("GroupNameUpdated", groupId, newName);
            }
        }

        public async Task AddGroupMember(string groupId, string userslist)
        {
            var stringvalues = await _messageService.UpdateGroupUsers(groupId, userslist);
            string stringobject = JsonConvert.SerializeObject(stringvalues);
            await Clients.All.SendAsync("AddedToGroupMember", stringobject);
        }

        public async Task RemoveGroupMember(Guid groupId, string memberId)
        {
            var userGroup = await _chatDBContext.UserGroups.FirstOrDefaultAsync(ug => ug.UserId == memberId && ug.GroupId == groupId);
            if (userGroup != null)
            {
                _chatDBContext.UserGroups.Remove(userGroup);
                await _chatDBContext.SaveChangesAsync();
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupId.ToString());
                await Clients.Client(Context.ConnectionId).SendAsync("RemovedFromGroup", groupId);
            }
        }

        public async Task SendMessage(string groupId, string senderId, string content)
        {
            var message = new Message
            {
                groupId = groupId,
                SenderId = senderId,
                Content = content,
                Timestamp = DateTime.Now
            };

            _chatDBContext.Messages.Add(message);
            await _chatDBContext.SaveChangesAsync();

            await Clients.Group(groupId.ToString()).SendAsync("NewMessageAdded", message);
        }

        public async Task SetStatus(string userId, string status)
        {
            UserStatuss userStatuss = await _messageService.UpdateUserStatus(userId, status);

            if (userStatuss != null)
            {
                await Clients.All.SendAsync("GetUpdateUserStatus", userStatuss.UserId, status);
            }
        }

        public async Task ShowHistoryOptions(Guid groupId, string memberId, string option)
        {
            var userGroup = await _chatDBContext.UserGroups.FirstOrDefaultAsync(ug => ug.UserId == memberId && ug.GroupId == groupId);
            if (userGroup != null)
            {
                switch (option)
                {
                    case "days":
                        int days = 7;
                        var cutoffDate = DateTime.UtcNow.AddDays(-days);
                        var messages = await _chatDBContext.Messages
                            .Where(m => m.groupId == groupId.ToString() && m.Timestamp >= cutoffDate)
                            .ToListAsync();
                        await Clients.Caller.SendAsync("HistoryOptionsSet", messages);
                        break;
                    case "all":
                        var allMessages = await _chatDBContext.Messages
                            .Where(m => m.groupId == groupId.ToString())
                            .ToListAsync();
                        await Clients.Caller.SendAsync("HistoryOptionsSet", allMessages);
                        break;
                    case "no":
                        await Clients.Caller.SendAsync("HistoryOptionsSet", null);
                        break;
                    default:
                        await Clients.Caller.SendAsync("HistoryOptionsError", "Invalid option.");
                        break;
                }
            }
            else
            {
                await Clients.Caller.SendAsync("HistoryOptionsError", "You are not a member of the group.");
            }
        }
        public override Task OnDisconnectedAsync(Exception? exp)
        {

            return base.OnDisconnectedAsync(exp);
        }
    }

}
