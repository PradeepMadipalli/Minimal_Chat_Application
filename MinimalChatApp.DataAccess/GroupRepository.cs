using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MinimalChatApp.DataAccess.Interface;
using MinimalChatApp.Model;
using MinimalChatApplication.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinimalChatApp.DataAccess
{
    public class GroupRepository : IGroupRepository
    {
        private readonly ChatDBContext _chatDBContext;
        private readonly UserManager<AppUser> _userManager;

        public GroupRepository(ChatDBContext chatDBContext, UserManager<AppUser> userManager)

        {
            _chatDBContext = chatDBContext;
            _userManager = userManager;
        }

        public async Task<ProfilePhoto> UploadPhoto(ProfilePhoto request)
        {

            ProfilePhoto profilePhoto = new ProfilePhoto()
            {
                userid = request.userid,
                PhotoPath = request.PhotoPath,
            };
            var profileexists = await _chatDBContext.ProfilePhoto.Where(a => a.userid == request.userid).FirstOrDefaultAsync();
            if (profileexists != null)
            {
                profileexists.PhotoPath = request.PhotoPath;
                await _chatDBContext.SaveChangesAsync();
            }
            else
            {
                await _chatDBContext.ProfilePhoto.AddAsync(profilePhoto);
                await _chatDBContext.SaveChangesAsync();
            }

            return profilePhoto;

        }
        public async Task<List<ProfilePhoto>> GetProfileDetails()
        {
            return await _chatDBContext.ProfilePhoto.ToListAsync();

        }
        public async Task<List<UserGroup>> GetGroupOfUsers(GroupUserRequest request)
        {
            return await _chatDBContext.UserGroups.Where(u => u.GroupId.ToString() == request.groupId).ToListAsync();
        }
        public async Task<AppUser> UpdateStatus(UpdateStatus request, string userid)
        {
            var user = await _userManager.FindByIdAsync(userid);
            if (user != null)
            {
                user.OnlineStatus = request.Status;

                await _userManager.UpdateAsync(user);
            }
            return user;

        }
        public async Task<UserStatuss> UpdateUserStatus(UserStatuss request)
        {
            UserStatuss user = await _chatDBContext.UserStatus.Where(u => u.UserId == request.UserId).SingleOrDefaultAsync();
            if (user != null)
            {
                user.UsersStatus = request.UsersStatus;

            }
            else
            {
                user = new UserStatuss()
                {
                    UserId = request.UserId,
                    UsersStatus = request.UsersStatus
                };
                await _chatDBContext.UserStatus.AddAsync(user);
            }
            await _chatDBContext.SaveChangesAsync();
            return user;

        }
        public async Task<List<OnlineStatus>> getOnlineStatus()
        {
            List<OnlineStatus> statuses = await _chatDBContext.OnlineStatus.ToListAsync();
            return statuses;
        }
        public async Task<List<UserGroup>> Deleteuserfromgroup(DeleteUsersFromGroup request)
        {
            List<UserGroup> usergroup = await _chatDBContext.UserGroups.Where(a => (a.UserId == request.userId && a.GroupId.ToString() == request.groupId)).ToListAsync();
            if (usergroup != null)
            {
                foreach (var item in usergroup)
                {
                    item.Status = 0;
                }
                await _chatDBContext.SaveChangesAsync();
            }
            return usergroup;

        }
        public async Task<Group> Editgroupname(EditGroupName request)
        {

            Group group = await _chatDBContext.Group.Where(a => a.GroupId.ToString() == request.groupId).FirstOrDefaultAsync();

            if (group != null)
            {
              group.GroupName = request.groupName;
               await _chatDBContext.SaveChangesAsync();
            }
            return group;
        }
    }
}
