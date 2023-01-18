using AuthWebApi.Models;

namespace AuthWebApi.Services.AuthService
{
    public interface IAuthService
    {
        Task<User> RegisterUser(UserDto userDto);
    }
}
