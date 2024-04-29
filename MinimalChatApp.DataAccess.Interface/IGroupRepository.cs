﻿using MinimalChatApp.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinimalChatApp.DataAccess.Interface
{
    public interface IGroupRepository
    {
        Task<ProfilePhoto> UploadPhoto(ProfilePhoto request);
        Task<List<ProfilePhoto>> GetProfileDetails();
        Task<List<UserGroup>> GetGroupOfUsers(GroupUserRequest groupId);
    }
}
