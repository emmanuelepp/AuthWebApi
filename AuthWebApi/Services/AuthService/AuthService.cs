using AuthWebApi.Models;

namespace AuthWebApi.Services.AuthService
{
    public class AuthService : IAuthService
    {
        public async Task<User> RegisterUser(UserDto userDto)
        {
            var user =  new User { UserName = userDto.UserName };

            return user;
        }
    }
}
