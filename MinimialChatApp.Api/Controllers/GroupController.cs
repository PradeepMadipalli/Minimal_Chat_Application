using Azure.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MinimalChatApp.Business;
using MinimalChatApp.Business.Interface;
using MinimalChatApp.DataAccess;
using MinimalChatApp.DataAccess.Interface;
using MinimalChatApp.Model;
using MinimalChatApplication.Model;
using System.Security.Claims;

namespace MinimialChatApp.Api.Controllers
{
    [Route("api/")]
    [ApiController]
    [Authorize]
    public class GroupController : ControllerBase
    {
        private readonly IGroupRepository _groupRepository;
        private readonly IGroupServices _groupServices;
        private readonly ChatDBContext chatDBContext;

        public GroupController(IGroupRepository groupRepository,IGroupServices groupServices,ChatDBContext chatDBContext)
        {
            this._groupRepository = groupRepository;
            this._groupServices = groupServices;
            this.chatDBContext = chatDBContext;
        }
        [HttpPost]
        [Route("creategroup")]
        public async Task<IActionResult> createGroup(RequestGroup group)
        {
            //var user = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var Group = new Group
            {
                GroupId=Guid.NewGuid(),
                GroupName=group.GroupName,
            };
            var usergroup = new UserGroup
            {
                UserId = group.UserId,
                GroupId = Group.GroupId,
                Status = 1
            };
            chatDBContext.Group.Add(Group);
            int numberOfChanges =  await chatDBContext.SaveChangesAsync();
            if(numberOfChanges > 0)
            {
                chatDBContext.UserGroups.Add(usergroup);
                await chatDBContext.SaveChangesAsync();
            }
            //Group groupp = await _groupServices.CreateGroup(group);
            return Ok(Group);
        }

       



    }
}
