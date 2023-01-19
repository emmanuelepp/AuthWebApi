using AuthWebApi.Data;
using AuthWebApi.Models;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;

namespace AuthWebApi.Services.AuthService
{
    public class AuthService : IAuthService
    {
        private readonly DataContext _dbContext;

        public AuthService(DataContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<User> RegisterUser(UserDto userDto)
        {
            CreatePasswordHash(userDto.Password, out byte[] passwordHash, out byte[] passwordSalt);
            var user = new User
            {
                UserName = userDto.UserName,
                PasswordHash = passwordHash,
                PasswordSalt = passwordSalt
            };

            _dbContext.Users.Add(user);
            await _dbContext.SaveChangesAsync();

            return user;
        }

        private bool VerifyPasswordHash(string password,  byte[] passwordHash,  byte[] passwordSalt) 
        {
            using (var hmac = new HMACSHA512(passwordSalt))
            {
                var hashValue = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
                return hashValue.SequenceEqual(passwordHash);
            }
        }

        private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using (var hmac = new HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));

            }
        }
    }
}
