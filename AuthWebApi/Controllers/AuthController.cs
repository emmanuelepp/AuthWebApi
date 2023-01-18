using AuthWebApi.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AuthWebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        [HttpPost]
        public async Task<ActionResult<User>> RegisterUSer(UserDto request) 
        {
            var user = new User {UserName = request.UserName};

         return Ok(user);
        }
    }
}
