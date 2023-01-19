using AuthWebApi.Data;
using AuthWebApi.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace AuthWebApi.Services.AuthService
{
    public class AuthService : IAuthService
    {
        private readonly DataContext _dbContext;
        private readonly IConfiguration _configuration; 

        public AuthService(DataContext dbContext, IConfiguration configuration)
        {
            _dbContext = dbContext;
            _configuration = configuration;
        }

        public async Task<AuthResponseDto> Login(UserDto userDto)
        {
            var user = await _dbContext.Users.FirstOrDefaultAsync( user => user.UserName == userDto.UserName );
            if (user == null)
            {
                return new AuthResponseDto { Message = "User not found."};
            }

            if (!VerifyPasswordHash(userDto.Password, user.PasswordHash, user.PasswordSalt)) 
            {
                return new AuthResponseDto { Message = "Wrong Password."};
            
            }
            string token = CreateToken(user);
            return new AuthResponseDto { Success = true, Token = token};
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

        private string CreateToken(User user) 
        {
            var claims = new List<Claim>()
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.UserName),
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
                _configuration.GetSection("AppSettings:Token").Value));

            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.Now.AddDays(1),
                signingCredentials: credentials);

            var jwt = new JwtSecurityTokenHandler().WriteToken(token);

            return jwt;
        }
    }
}
