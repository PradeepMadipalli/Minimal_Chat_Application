
using Microsoft.AspNetCore.Identity;
using MinimalChatApp.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinimalChatApp.DataAccess.Interface
{
    public interface ILoginRepository
    {

        Task<object> GetUsersList();
        Task<AppUser> userFindByEmail(string email);

        Task<bool> userCheckPassword(AppUser users, string password);

        void userCreate(AppUser identityUser, string password);

        Task<string> userGetUserId(AppUser identityUser);
    }
}
