
using Microsoft.AspNetCore.Identity;
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
        Task<IdentityUser> userFindByEmail(string email);

        Task<bool> userCheckPassword(IdentityUser users, string password);

        void userCreate(IdentityUser identityUser, string password);

        Task<string> userGetUserId(IdentityUser identityUser);
    }
}
