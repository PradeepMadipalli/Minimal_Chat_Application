
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MinimalChatApp.DataAccess.Interface;
using MinimalChatApp.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinimalChatApp.DataAccess
{
    public class LoginRepository : ILoginRepository
    {
        private readonly UserManager<AppUser> _userManager;
        public LoginRepository(UserManager<AppUser> userManager)
        {
            _userManager = userManager;
        }
        public async Task<object> GetUsersList()
        {
            return await _userManager.Users.ToListAsync();
        }

        public async Task<AppUser> userFindByEmail(string email)
        {
            if (email == null) throw new ArgumentNullException("email");
            else {
                return await _userManager.FindByEmailAsync(email);
            }
           
        }

        public async Task<bool> userCheckPassword(AppUser users, string password)
        {
            return await _userManager.CheckPasswordAsync(users, password);
        }

        public async void userCreate(AppUser identityUser, string password)
        {
            await _userManager.CreateAsync(identityUser, password);
        }

        public async Task<string> userGetUserId(AppUser identityUser)
        {
            return await _userManager.GetUserIdAsync(identityUser);
        }
    }
}
