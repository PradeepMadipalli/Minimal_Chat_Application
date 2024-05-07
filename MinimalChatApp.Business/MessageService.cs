
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

using Microsoft.EntityFrameworkCore;
using Microsoft.Data.SqlClient;
using Microsoft.AspNetCore.Http;
using MinimalChatApp.Model;
using Newtonsoft.Json;
using Azure.Core;
using System.Collections;
using System.Text.RegularExpressions;

namespace MinimalChatApp.Business
{
    public class MessageService : IMessageService
    {
        private readonly IMessageRepository _messagerepository;
        private readonly ChatDBContext _chatDBContext;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILoginRepository _loginRepository;

        public MessageService(IMessageRepository messageRepository, ChatDBContext chatDBContext, IHttpContextAccessor httpContextAccessor, ILoginRepository loginRepository)
        {
            _messagerepository = messageRepository;
            _chatDBContext = chatDBContext;
            this._httpContextAccessor = httpContextAccessor;
            _loginRepository = loginRepository;
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
            var messagesQuery = _chatDBContext.Messages.AsQueryable();

            if (request.groupId == null || request.groupId == "")
            {
                messagesQuery = messagesQuery.Where(m => (m.SenderId == userId && m.ReceiverId == request.UserId) || (m.SenderId == request.UserId && m.ReceiverId == userId) || (m.SenderId == request.UserId && m.groupId == userId))
               .Where(m => m.Timestamp < beforeDate)
               .OrderBy(m => sortOrder == SortOrder.Ascending ? m.Timestamp : (DateTime?)null)
               .OrderByDescending(m => sortOrder == SortOrder.Descending ? m.Timestamp : (DateTime?)null)
               .Take(request.Count);
            }
            else
            {
                UserGroup group = await _messagerepository.GetUserGroupdate(userId, request.groupId);
                if (group != null)
                {
                    if (group.AddDays > 0)
                    {
                        DateTime newDate=  group.Createdate.AddDays(-group.AddDays);
                        messagesQuery = messagesQuery.Where(m => (m.groupId == request.groupId && m.Timestamp > newDate));
                    }
                    else if (group.AddDays == 0)
                    {
                        messagesQuery = messagesQuery.Where(m => (m.groupId == request.groupId && m.Timestamp > group.Createdate));
                    }
                    else
                    {
                        messagesQuery = messagesQuery.Where(m => m.groupId == request.groupId);
                    }
                }

            }



            List<Message> messages = await messagesQuery.Select(m => new Message
            {
                MessageId = m.MessageId,
                SenderId = m.SenderId,
                ReceiverId = m.ReceiverId,
                Content = m.Content,
                Timestamp = m.Timestamp,
                groupId = m.groupId,
                ShowOptions = m.ShowOptions,
                GifImageId = m.GifImageId,
                ThreadMessage = m.ThreadMessage,

            }).ToListAsync();
            return messages;
        }
        public async Task<Message> sendMessage(MessageRequest request, string user)
        {

            if (request.ReceiverId == "")
            {
                request.ReceiverId = null;
            }
            if (request.groupId == "")
            {
                request.groupId = null;
            }
            var message = new Message
            {
                MessageId = Guid.NewGuid(),
                SenderId = user,
                ReceiverId = request.ReceiverId,
                Content = request.Content,
                Timestamp = DateTime.UtcNow,
                groupId = request.groupId,
                ShowOptions = request.ShowOptions,
                GifImageId = request.GifImageId,
                ThreadMessage = request.ThreadMessage,


            };
          

            _chatDBContext.Messages.Add(message);
            await _chatDBContext.SaveChangesAsync();
            return message;
        }
        public async Task<Insertmessage> sendMessage(ChatMessageRequest request)
        {

            if (request.ReceiverId == "" || request.ReceiverId == null)
            {
                request.ReceiverId = null;
            }

            if (request.ThreadMessage == "" || request.ThreadMessage == null)
            {
                request.ThreadMessage = null;
            }
            var message = new Message
            {
                MessageId = Guid.NewGuid(),
                SenderId = request.SenderId,
                ReceiverId = request.ReceiverId,
                Content = request.Content,
                Timestamp = DateTime.UtcNow,
                groupId = request.groupId,
                ShowOptions = 0,
                GifImageId = request.GifImageId,
                ThreadMessage = request.ThreadMessage,
                

            };

            var message1 = new Insertmessage
            {
                messageId = message.MessageId,
                senderId = request.SenderId,
                receiverId = request.ReceiverId,
                content = request.Content,
                timestamp = DateTime.UtcNow,
                groupId = request.groupId,
                showOptions = 0,
                gifImageId = request.GifImageId,
                threadMessage = request.ThreadMessage,

            };
            await _messagerepository.sendMessage(message);
            return message1;
        }

        public async Task<List<GetGroups>> GetGetGroups()
        {
            var groups = await _chatDBContext.Group.ToListAsync();


            List<GetGroups> GroupDetails = new List<GetGroups>();

            foreach (var group in groups)
            {
                GetGroups v = new GetGroups()
                {
                    GroupId = group.GroupId.ToString(),
                    GroupName = group.GroupName
                };
                GroupDetails.Add(v);
            }
            Groupss groupss = new Groupss()
            {
                Groupsss = GroupDetails
            };
            return GroupDetails;
        }
        public string GetNameIdentifier()
        {
            // Get the current HTTP context
            HttpContext context = _httpContextAccessor.HttpContext;

            // Get the current user's claims principal
            ClaimsPrincipal user = context.User;

            // Find the NameIdentifier claim
            Claim nameIdentifierClaim = user.FindFirst(ClaimTypes.NameIdentifier);

            // Check if the NameIdentifier claim exists
            if (nameIdentifierClaim != null)
            {
                // Get the value of the NameIdentifier claim
                string nameIdentifier = nameIdentifierClaim.Value;
                return nameIdentifier;
            }

            return null; // NameIdentifier claim not found
        }
        public async Task<MinimalChatApp.Model.Group> CreateGroup(string groupname, string userlist, string userid)
        {
            List<UserList> request = JsonConvert.DeserializeObject<List<UserList>>(userlist);
            await _messagerepository.insertGruop(groupname);
            MinimalChatApp.Model.Group group = await _messagerepository.FindByGroupName(groupname);

            List<UserGroup> groups = new List<UserGroup>();
            foreach (var item in request)
            {
                UserGroup userGroup = new UserGroup()
                {
                    UserId = item.UserId,
                    GroupId = group.GroupId,
                    Status = 1,
                    Createdate = DateTime.UtcNow,

                };
                groups.Add(userGroup);

                Message message = new Message
                {
                    MessageId = Guid.NewGuid(),
                    SenderId = userid,
                    ReceiverId = item.UserId,
                    Content = item.UserName + " new group member add to Group",
                    Timestamp = DateTime.UtcNow,
                    groupId = group.GroupId.ToString(),
                    ShowOptions = 1,
                };
                await _messagerepository.sendMessage(message);
            }

            await _messagerepository.InsertUserGroup(groups);


            return group;
        }

        public async Task<UserStatuss> UpdateUserStatus(string userId, int status)
        {
            UserStatuss userStatuss = new UserStatuss
            {
                UserId = userId,
                UsersStatus = status
            };
            //var user = await _loginRepository.userFindById(userId);
            //if (user != null)
            //{

            //    UserStatuss userStatuss1 = await _messagerepository.UpdateUserStatus(userStatuss);

            //}
            return userStatuss;
        }
        public async Task<object> UpdateGroupUsers(string groupId, string userslist, string userid)
        {
            List<UserList> request = JsonConvert.DeserializeObject<List<UserList>>(userslist);
            List<UserGroup> groups = new List<UserGroup>();
            foreach (var item in request)
            {
                UserGroup userGroup = new UserGroup()
                {
                    UserId = item.UserId,
                    GroupId = Guid.Parse(groupId),
                    Status = 1,
                    Createdate = DateTime.UtcNow,

                };
                groups.Add(userGroup);
            }
            foreach (var item in request)
            {

                Message message = new Message
                {
                    MessageId = Guid.NewGuid(),
                    SenderId = userid,
                    ReceiverId = item.UserId,
                    Content = item.UserName + " new group member add to Group",
                    Timestamp = DateTime.UtcNow,
                    groupId = groupId,
                    ShowOptions = 1,
                };
                await _messagerepository.sendMessage(message);
            }

            await _messagerepository.InsertUserGroup(groups);
            return new { GroupId = groupId, userGrouplist = request };

            //return new { groupId =groupId,userGrouplist=request};
        }
        public async Task<List<UserGroup>> GetUserGroup(string UserId)
        {
            List<UserGroup> userGroups = await _messagerepository.GetUserGroup(UserId);
            return userGroups;
        }
        public async Task<int> GetUserStatus(string userId)
        {
            int status = 0;

            AppUser user = await _messagerepository.GetUserStatus(userId);
            if (user != null)
            {
                status = user.OnlineStatus;
            }
            return status;
        }
        public async Task<UserStatuss> UpdateStatus(string userId, int status)
        {
            UserStatuss user = await _messagerepository.UpdateUserStatus(userId, status);
            return user;
        }
        public async Task<Message> ShowOptions(string noofdays,string messageId,string senderId)
        {

           Message message = await _messagerepository.FindMessage(messageId);
            UserGroup group = await _messagerepository.GetUserGroupdate(senderId, message.groupId);
            if (message != null)
            {
                message.ShowOptions = 0;
            }
            if(group!= null)
            {
                group.AddDays=Convert.ToInt32(noofdays);
            }

            await _messagerepository.ShowOptionUpdate(message);
            return message;
        }

    }

}
