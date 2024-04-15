
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MinimalChatApp.DataAccess.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinimalChatApp.DataAccess
{
    public class LoginRepository : ILoginRepository
    {
        private readonly UserManager<IdentityUser> _userManager;
        public LoginRepository(UserManager<IdentityUser> userManager)
        {
            _userManager = userManager;
        }
        public async Task<object> GetUsersList()
        {
            return await _userManager.Users.ToListAsync();
        }

        public async Task<IdentityUser> userFindByEmail(string email)
        {
            if (email == null) throw new ArgumentNullException("email");
            else {
                return await _userManager.FindByEmailAsync(email);
            }
           
        }

        public async Task<bool> userCheckPassword(IdentityUser users, string password)
        {
            return await _userManager.CheckPasswordAsync(users, password);
        }

        public async void userCreate(IdentityUser identityUser, string password)
        {
            await _userManager.CreateAsync(identityUser, password);
        }

        public async Task<string> userGetUserId(IdentityUser identityUser)
        {
            return await _userManager.GetUserIdAsync(identityUser);
        }
    }
}
