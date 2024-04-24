using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using MinimalChatApp.Business;
using MinimalChatApp.Business.Interface;
using MinimalChatApp.Model;
using MinimalChatApplication.Model;
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

        public LoginController(IConfiguration configuration, UserManager<AppUser> userManager, ILoginServices loginServices,IMessageService messageService
            )
        {
            _Configuration = configuration;
            _userManager = userManager;
            _loginsevice = loginServices;
            _messageService = messageService;
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

            List<GetUsers> users = await _loginsevice.GetGetUsers();

            List<GetGroups> Groups = await _messageService.GetGetGroups();
            if (users == null || Groups==null)
            {
                StatusCode(404, new { error = "Users not found" });
            }

            return Ok(new { users, Groups });
        }

    }
}

