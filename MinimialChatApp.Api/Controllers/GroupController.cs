using Azure.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
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
        private readonly UserManager<AppUser> _userManager;

        public GroupController(IGroupRepository groupRepository, IGroupServices groupServices,
            ChatDBContext chatDBContext, IWebHostEnvironment webHostEnvironment, ILoginServices loginServices, UserManager<AppUser> userManager)
        {
            this._groupRepository = groupRepository;
            this._groupServices = groupServices;
            this.chatDBContext = chatDBContext;
            this._webHostEnvironment = webHostEnvironment;
            this._loginServices = loginServices;
            _userManager = userManager;
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
                Status = 1,
            };
            chatDBContext.Group.Add(Group);
            int numberOfChanges = await chatDBContext.SaveChangesAsync();
            if (numberOfChanges > 0)
            {
                chatDBContext.UserGroups.Add(usergroup);
                await chatDBContext.SaveChangesAsync();
            }
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
            string filepath1 = @"D:\\MinimalAngular\\MinimalChatAppRealTimeCommunication\\src\\assets\";
            var random = DateTime.UtcNow.Ticks;
            string photoPath = random + photoFile.FileName;
            var filePath = Path.Combine(filepath1, photoPath);
            string filepath2 = @"\assets\" + photoPath;

            string filexc = Path.GetExtension(photoFile.FileName);
            ProfilePhoto profilePhoto = new ProfilePhoto
            {
                userid = user,
                PhotoPath = filepath2,
            };
            await _groupRepository.UploadPhoto(profilePhoto);
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await photoFile.CopyToAsync(stream);
            }
            return Ok(profilePhoto);
        }


        [HttpPost]
        [Route("GetuserGroups")]
        public async Task<IActionResult> GetuserGroups(GroupUserRequest groupid)
        {
            List<UserGroup> userGroups = await _groupServices.GetGroupOfUsers(groupid);
            List<GetUsers> userss = await _loginServices.GetGetUsers();
            var gofus = userGroups.Join(userss, u => u.UserId, ug => ug.UserId, (u, ug) => new
            {
                userId = u.UserId,
                userName = ug.UserName,
                groupId = u.GroupId,
                status = u.Status,
            }).ToList();
            var gofu = gofus.Distinct().Where(a => a.status == 1).ToList();
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

            var user = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

             
            List<OnlineStatus> onlineStatuses = await _groupServices.GetStsatus();
            AppUser appUser = await _userManager.FindByIdAsync(user);
            if(appUser != null)
            {
                try
                {
                    foreach (var item in onlineStatuses)
                    {
                        if (item.Status.ToLower().Trim() != appUser.OnlineStatus.ToLower().Trim())
                        {
                            OnlineStatus onlineStatus = new OnlineStatus
                            {
                                Id = onlineStatuses.Count + 1,
                                Status = appUser.OnlineStatus
                            };
                            onlineStatuses.Add(onlineStatus);
                            break;
                        }


                    }
                }catch (Exception ex)
                {
                    throw ex;
                }

            }
            return Ok(onlineStatuses);
        }
        [HttpPost]
        [Route("updateStatus")]
        public async Task<IActionResult> UpdateStatus(UpdateStatus status)
        {
            {
                var user = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                AppUser appUser = null;

                if (user != null)
                {
                   appUser= await _groupServices.UpdateStatuss(status, user);
                }
                return Ok(appUser);
            }

        }
        [HttpPost]
        [Route("delete")]
        public async Task<IActionResult> DeleteUserFromGroup(DeleteUsersFromGroup request)
        {
            List<UserGroup> userGroupss = await _groupServices.DeleteUserfromGroup(request);
            return Ok(userGroupss);
        }
        [HttpPost]
        [Route("editgroupname")]
        public async Task<IActionResult> EditGroupName(EditGroupName request)
        {
            Group group = await _groupServices.EdituserGroup(request);
            return Ok(group);
        }
    }
}
