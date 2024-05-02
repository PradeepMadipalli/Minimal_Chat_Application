using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MinimalChatApp.Business.Interface;
using MinimalChatApp.DataAccess.Interface;
using MinimalChatApp.Model;
using MinimalChatApplication.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinimalChatApp.Business
{
    public class GroupServices : IGroupServices
    {
        private readonly IGroupRepository _groupRepository;

   

        public GroupServices(IGroupRepository groupRepository)
        {
            _groupRepository = groupRepository;
        }

        public async Task<Group> CreateGroup(RequestGroup groupp)
        {
            return null;
        }

        public Task<List<GetGroups>> GetGetGroups()
        {
            throw new NotImplementedException();
        }

        public async Task<Group> JoinGroup(RequestGroup groupp)
        {
            var group = new Group()
            {

                GroupName = groupp.GroupName
            };
            //_chatDBContext.Group.Add(group);
            //await _chatDBContext.SaveChangesAsync();
            return group;
        }

        public async Task<List<UserGroup>> GetGroupOfUsers(GroupUserRequest groupId)
        {
            List<UserGroup> userGroups = await _groupRepository.GetGroupOfUsers(groupId);
            return userGroups;
        }


       public async Task<List<OnlineStatus>> GetStsatus()
        {
            List<OnlineStatus> onlines = await _groupRepository.getOnlineStatus();   
            return onlines;
        }
        public async Task<AppUser> UpdateStatuss(UpdateStatus status, string userid)
        {
            AppUser user = await _groupRepository.UpdateStatus(status, userid);

            return user;
            
        }

    }
}
