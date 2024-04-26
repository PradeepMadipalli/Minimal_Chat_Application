using MinimalChatApp.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinimalChatApp.DataAccess.Interface
{
    public interface IGroupRepository
    {
        Task UploadPhoto(ProfilePhoto profilePhoto);
        Task<List<ProfilePhoto>> GetProfileDetails();
    }
}
