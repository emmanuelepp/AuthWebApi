using AuthWebApi.Models;
using AuthWebApi.Services.AuthService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AuthWebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost]
        public async Task<ActionResult<User>> RegisterUSer(UserDto userDto) 
        {
            var response = await _authService.RegisterUser(userDto);

         return Ok(response);

        }

        [HttpPost("login")]
        public async Task<ActionResult<User>> Login(UserDto userDto)
        {
            var response = await _authService.Login(userDto);
            if (response.Success)
                return Ok(response);

            return BadRequest(response);
        }

        [HttpPost("refresh-token")]
        public async Task<ActionResult<string>> RefreshToken()
        {
            var response = await _authService.RefreshToken();
            if (response.Success)
                return Ok(response);

            return BadRequest(response);
        }


        [HttpGet, Authorize]
        public ActionResult<string> ValidateUserAuthorization() 
        {
            return Ok("You're authorized!"); 
        }
    }
}
