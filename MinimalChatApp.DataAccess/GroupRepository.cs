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

        public GroupRepository(ChatDBContext chatDBContext)

        {
            this._chatDBContext = chatDBContext;
        }

        public async Task<ProfilePhoto> UploadPhoto(ProfilePhoto request)
        {

            ProfilePhoto profilePhoto = new ProfilePhoto()
            {
                userid = request.userid,
                PhotoPath = request.PhotoPath,
            };
            var profileexists = await _chatDBContext.ProfilePhoto.Where(a=>a.userid == request.userid).FirstOrDefaultAsync();
            if(profileexists != null)
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
    }
}
