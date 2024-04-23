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
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;

namespace MinimalChatApp.Common
{
    public sealed class ChatHub : Hub
    {
        private readonly ChatDBContext chatDBContext;
        private readonly IMessageService _messageService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IDictionary<string, UserGroupConnection> _connection;
        private readonly IMessageRepository _messageRepository;
        

        public ChatHub(ChatDBContext chatDBContext, IMessageRepository messageRepository, 
            IMessageService messageService, IHttpContextAccessor httpContextAccessor,IDictionary<string,UserGroupConnection> connection)
        {
            this.chatDBContext = chatDBContext;
            this._messageService = messageService;
            this._httpContextAccessor = httpContextAccessor;
            this._connection = connection;
            this._messageRepository = messageRepository;
        }
        public async Task sendMessages(string messagess)
        {
            try
            {
                
                ChatMessageRequest request = JsonConvert.DeserializeObject<ChatMessageRequest>(messagess);
                var receiverExists = await _messageRepository.isExistReceiver(request.ReceiverId);
                if (receiverExists)
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

        public async Task JoinRoom(UserGroupConnection userConnection)
        {
            await Groups.AddToGroupAsync(userConnection.senderId, userConnection.Group!);
            await Clients.Group(userConnection.Group!)
                .SendAsync("ReceiveMessage", "Lets Program Bot", $"{userConnection.User} has Joined the Group", DateTime.Now);
        }
        //public async Task SendMessage(string message)
        //{
        //    UserGroupConnection request = JsonConvert.DeserializeObject<UserGroupConnection>(message);
        //    var receiverExists = await _messageRepository.isExistReceiver(request.senderId);
        //    if (receiverExists, out UserGroupConnection userGroupConnection);
        //    {
        //        await Clients.Group(userGroupConnection.Group!)
        //            .SendAsync("ReceiveMessage", userGroupConnection.User, message, DateTime.Now);
        //    }
        //}
        public override Task OnDisconnectedAsync(Exception? exp)
        {
            if (!_connection.TryGetValue(Context.ConnectionId, out UserGroupConnection roomConnection))
            {
                return base.OnDisconnectedAsync(exp);
            }

            _connection.Remove(Context.ConnectionId);
            Clients.Group(roomConnection.Group!)
                .SendAsync("ReceiveMessage", "Lets Program bot", $"{roomConnection.User} has Left the Group", DateTime.Now);
            SendConnectedUser(roomConnection.Group!);
            return base.OnDisconnectedAsync(exp);
        }
        public Task SendConnectedUser(string group)
        {
            var users = _connection.Values
                .Where(u => u.Group == group)
                .Select(s => s.User);
            return Clients.Group(group).SendAsync("ConnectedUser", users);
        }

    }

}
