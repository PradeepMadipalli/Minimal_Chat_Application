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

        public async Task UploadPhoto(ProfilePhoto profilePhoto)
        {
            await _chatDBContext.ProfilePhoto.AddAsync(profilePhoto);
            await _chatDBContext.SaveChangesAsync();
        }
        public async Task<List<ProfilePhoto>> GetProfileDetails()
        {
           return await _chatDBContext.ProfilePhoto.ToListAsync();

        }
    }
}
