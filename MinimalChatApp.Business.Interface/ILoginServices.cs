
using Microsoft.AspNetCore.Identity;
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
        
        TokenResponse GetTokenResponse(Users users, IdentityUser user);
        Task<ResponseRegister> GetResponseRegister(IdentityUser user, Register register);
    }
}
