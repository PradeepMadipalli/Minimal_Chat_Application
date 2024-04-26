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
        private readonly IWebHostEnvironment _webHostEnvironment;

        public GroupController(IGroupRepository groupRepository,IGroupServices groupServices,
            ChatDBContext chatDBContext,IWebHostEnvironment webHostEnvironment)
        {
            this._groupRepository = groupRepository;
            this._groupServices = groupServices;
            this.chatDBContext = chatDBContext;
            this._webHostEnvironment = webHostEnvironment;
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
        [HttpPost]
        [Route("photo")]
        public async Task<IActionResult> UploadProfilePhoto(IFormFile photoFile)
        {
            var user = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (photoFile == null || photoFile.Length == 0)
            {
                return BadRequest("No file uploaded");
            }

            var filePath = Path.Combine(_webHostEnvironment.WebRootPath, "Uploads", photoFile.FileName);
            ProfilePhoto profilePhoto = new ProfilePhoto
            {
                userid=user,
                PhotoPath=filePath,
            };
            await _groupRepository.UploadPhoto(profilePhoto);
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await photoFile.CopyToAsync(stream);
            }
            return Ok(filePath);
        }







    }
}
