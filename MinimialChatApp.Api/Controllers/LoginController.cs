using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using MinimalChatApp.Business;
using MinimalChatApp.Business.Interface;
using MinimalChatApp.DataAccess.Interface;
using MinimalChatApp.Model;
using MinimalChatApplication.Model;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace MinimialChatApp.Api.Controllers
{
    [Route("api/")]
    [ApiController]
    [EnableCors("AllowOrigin")]
    public class LoginController : ControllerBase
    {
        private readonly IConfiguration _Configuration;
        private readonly UserManager<AppUser> _userManager;
        private readonly ILoginServices _loginsevice;
        private readonly IMessageService _messageService;
        private readonly IGroupRepository _groupRepository;

        public LoginController(IConfiguration configuration, UserManager<AppUser> userManager, ILoginServices loginServices, IMessageService messageService, IGroupRepository groupRepository
            )
        {
            _Configuration = configuration;
            _userManager = userManager;
            _loginsevice = loginServices;
            _messageService = messageService;
            _groupRepository = groupRepository;
        }


        [HttpPost]
        [Route("Register")]
        [AllowAnonymous]
        public async Task<IActionResult> Register([FromBody] Register register)
        {
            if (ModelState.IsValid)
            {


                var userExist = await _userManager.FindByEmailAsync(register.Email);
                if (userExist != null)
                {
                    return StatusCode(StatusCodes.Status409Conflict, new Response { Status = "Error", Message = "User Already Exist" });
                }
                AppUser user = new()
                {
                    Email = register.Email,
                    UserName = register.Name,
                    SecurityStamp = Guid.NewGuid().ToString(),
                    OnlineStatus = 1
                };
                var result = await _userManager.CreateAsync(user, register.Password);
                if (result.Succeeded)
                {



                    ResponseRegister responseRegister = await _loginsevice.GetResponseRegister(user, register);

                    return Ok(responseRegister);

                }
                else
                {
                    return BadRequest("Registration failed due to validation errors");
                    //return StatusCode(result.Errors.Count); ;

                }
            }
            else
            {

                return BadRequest("Registration failed due to validation errors");

            }

        }
        [HttpPost]
        [Route("Login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] Users users)
        {
            var user = await _userManager.FindByEmailAsync(users.Email);
            if (user != null && await _userManager.CheckPasswordAsync(user, users.Password))
            {
                TokenResponse response = _loginsevice.GetTokenResponse(user);
                return Ok(response);
            }
            return Unauthorized();
        }



        [HttpGet]
        [Route("users")]
        public async Task<IActionResult> GetUsers()
        {
            var user = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            List<GetUsers> userss = await _loginsevice.GetGetUsers();
            List<ProfilePhoto> profilePhotos = await _groupRepository.GetProfileDetails();
            List<GetGroups> Groupss = await _messageService.GetGetGroups();
            List<UserGroup> userGroups = await _messageService.GetUserGroup(user);
            var users = userss.GroupJoin(profilePhotos, a => a.UserId, b => b.userid, (a, b) => new { a, b })
                .SelectMany(a => a.b.DefaultIfEmpty(), (ur, ph) => new
                {
                    UserId = ur.a.UserId,
                    UserName = ur.a.UserName,
                    UserEmail = ur.a.UserEmail,
                    PhotoPath = ph?.PhotoPath == null ? null : ph.PhotoPath,
                }).ToList();
            List<GetGroups> getGroups = Groupss.Join(userGroups, u => u.GroupId, g => g.GroupId.ToString(), (u, g) => new GetGroups
            {
                GroupId = g.GroupId.ToString(),
                GroupName = u.GroupName
            }).ToList();
            if (users == null || Groupss == null)
            {
                StatusCode(404, new { error = "Users not found" });
            }

            return Ok(new { users, getGroups });
        }

    }
}

