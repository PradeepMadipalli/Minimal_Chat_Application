﻿using MinimalChatApp.Model;
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

        Task<Group> insertGruop(string groupname);
        Task<List<UserGroup>> InsertUserGroup(List<UserGroup> usergroup);
        Task<Group> FindByGroupName(string groupname);
        Task<UserStatuss> UpdateUserStatus(string userId, int status);

        Task<List<UserGroup>> GetUserGroup(string UserId);
        Task<AppUser> GetUserStatus(string userId);

        Task<Message> sendMessage(Message message);
        Task<UserGroup> GetUserGroupdate(string UserId,string groupid);
        Task<Message> ShowOptionUpdate(Message message);


    }
}
