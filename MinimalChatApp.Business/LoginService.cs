
using Microsoft.IdentityModel.Tokens;
using MinimalChatApp.Business.Interface;
using MinimalChatApplication.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Identity;
using MinimalChatApp.DataAccess.Interface;
using Microsoft.EntityFrameworkCore;
using MinimalChatApp.Model;
using System.IdentityModel.Tokens.Jwt;


namespace MinimalChatApp.Business
{
    public class LoginService : ILoginServices
    {
        private readonly IConfiguration _Configuration;
        private readonly UserManager<AppUser> _userManager;
        private readonly ILoginRepository _loginRepository;
        public LoginService(IConfiguration configuration, UserManager<AppUser> userManager,ILoginRepository loginRepository
            )
        {
            _Configuration = configuration;
            _userManager = userManager;
            _loginRepository=loginRepository;
        }
        private JwtSecurityToken GenerateToken(List<Claim> claimss)
        {

           
            var securitykey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_Configuration["Jwt:Key"]));
            var credentials = new SigningCredentials(securitykey, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(_Configuration["Jwt:Issuer"],
                _Configuration["Jwt:Audience"],
                claims: claimss,
                expires: DateTime.Now.AddMinutes(15),
                signingCredentials: credentials);


            return token;
        }

        public async Task<List<GetUsers>> GetGetUsers()
        {
            var users = await _userManager.Users.ToListAsync();

            List<GetUsers> UserDetails = new List<GetUsers>();

            foreach (var user in users)
            {
                GetUsers v = new GetUsers()
                {
                    UserId = user.Id,
                    UserEmail = user.Email,
                    UserName = user.UserName
                };
                UserDetails.Add(v);
            }
            Userss userss = new Userss()
            {
                Userrs = UserDetails
            };
            return UserDetails;
        }

        public async  Task<ResponseRegister> GetResponseRegister(AppUser user, Register register)
        {
            string usid = await _loginRepository.userGetUserId(user);
            ResponseRegister responseRegister = new ResponseRegister()
            {
                userid = usid,
                username = register.Name,
                Email = register.Email,
            };
            return responseRegister;
        }

        public TokenResponse GetTokenResponse(AppUser user)
        {

            var authclaims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
                };
            var JWTtoken = GenerateToken(authclaims);

            Profile profile = new Profile()
            {
                UId = user.Id,
                Email = user.Email,
                Name = user.UserName
            };

            TokenResponse response = new TokenResponse()
            {
                Token = new JwtSecurityTokenHandler().WriteToken(JWTtoken),
                Profiles = profile
            };
            return response;
        }

    }
}
