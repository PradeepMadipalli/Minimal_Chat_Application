using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MinimalChatApp.Business.Interface;
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
        private readonly ChatDBContext _chatDBContext;

        public GroupServices(ChatDBContext chatDBContext)
        {
            this._chatDBContext = chatDBContext;
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
            _chatDBContext.Group.Add(group);
            await _chatDBContext.SaveChangesAsync();
            return group;
        }
                    
    }
}
