using Azure.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MinimalChatApp.Business;
using MinimalChatApp.Business.Interface;
using MinimalChatApp.DataAccess;
using MinimalChatApp.DataAccess.Interface;
using MinimalChatApp.Model;
using MinimalChatApplication.Model;
using System.Collections.Generic;
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
        private readonly ILoginServices _loginServices;

        public GroupController(IGroupRepository groupRepository, IGroupServices groupServices,
            ChatDBContext chatDBContext, IWebHostEnvironment webHostEnvironment, ILoginServices loginServices)
        {
            this._groupRepository = groupRepository;
            this._groupServices = groupServices;
            this.chatDBContext = chatDBContext;
            this._webHostEnvironment = webHostEnvironment;
            this._loginServices = loginServices;
        }
        [HttpPost]
        [Route("creategroup")]
        public async Task<IActionResult> createGroup(RequestGroup group)
        {
            //var user = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var Group = new Group
            {
                GroupId = Guid.NewGuid(),
                GroupName = group.GroupName,
            };
            var usergroup = new UserGroup
            {
                UserId = group.UserId,
                GroupId = Group.GroupId,
                Status = 1
            };
            chatDBContext.Group.Add(Group);
            int numberOfChanges = await chatDBContext.SaveChangesAsync();
            if (numberOfChanges > 0)
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
                userid = user,
                PhotoPath = filePath,
            };
            await _groupRepository.UploadPhoto(profilePhoto);
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await photoFile.CopyToAsync(stream);
            }
            return Ok(filePath);
        }


        [HttpPost]
        [Route("GetuserGroups")]
        public async Task<IActionResult> GetuserGroups(GroupUserRequest groupid)
        {
            List<UserGroup> userGroups = await _groupServices.GetGroupOfUsers(groupid);
            List<GetUsers> userss = await _loginServices.GetGetUsers();
            var gofu = userGroups.Join(userss, u => u.UserId, ug => ug.UserId, (u, ug) => new
            {
                userId = u.UserId,
                userName = ug.UserName,
                groupId = u.GroupId
            }).ToList();
            return Ok(gofu);
        }
        //[HttpPost]
        //[Route("status/update")]
        //[Authorize]
        //public async Task<IActionResult> UpdateStatus(StatusUpdateModel model)
        //{

        //    if (!User.Identity.IsAuthenticated)
        //    {
        //        return Unauthorized();
        //    }
        //    var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        //    if (userId == null)
        //    {
        //        return Unauthorized();
        //    }
        //    var user = await _groupServices.GetUser(userId,model);
        //    if (user == null)
        //    {
        //        return NotFound();
        //    }
        //    user.Status = model.Status;
        //    try
        //    {
        //        await _dbContext.SaveChangesAsync();
        //    }
        //    catch (Exception)
        //    {
        //        return StatusCode(500);
        //    }
        //    return Ok();
        //}

        [HttpGet]
        [Route("getStatus")]
        public async Task<IActionResult> GetOnlineStatus()
        {
            List<OnlineStatus> onlineStatuses = await _groupServices.GetStsatus();
            return Ok(onlineStatuses);
        }
        [HttpPost]
        [Route("updateStatus")]
        public async Task<IActionResult> UpdateStatus(UpdateStatus status)
        {
            {
                var user = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                if (user != null)
                {
                    await _groupServices.UpdateStatuss(status, user);
                }
                return Ok("Success");
            }

        }
    }
}
