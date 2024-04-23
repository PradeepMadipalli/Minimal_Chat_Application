
using Microsoft.AspNetCore.Identity;
using MinimalChatApp.Model;
using MinimalChatApplication.Model;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace MinimalChatApp.Business.Interface
{
    public interface ILoginServices
    {
        Task<List<GetUsers>> GetGetUsers();
        
        TokenResponse GetTokenResponse(AppUser user);
        Task<ResponseRegister> GetResponseRegister(AppUser user, Register register);
    }
}
