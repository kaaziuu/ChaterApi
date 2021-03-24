using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Chater.Models;
using Chater.Repository.Abstract;
using Chater.Service.Abstract;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace Chater.Service.Concrete
{
    public class IdentityService : IIdentityService
    {
        private readonly IUserRepository _userRepository;
        private string key;
        
        public IdentityService(IUserRepository userRepository, IConfiguration configuration)
        {
            _userRepository = userRepository;
            key = configuration.GetSection("JwtSettings")["secret"];
        }


        public async Task<string> AuthenticateAsync(string username, string password)
        {
            var user = await _userRepository.GetUserByUsernameAsync(username);
            if (user is null)
                return null;

            if (!BCrypt.Net.BCrypt.Verify(password, user.Password))
            {
                return null;
            }
            
            var tokenHandler = new JwtSecurityTokenHandler();

            var tokenKey = Encoding.ASCII.GetBytes(key);

            var tokenDescriptor = new SecurityTokenDescriptor()
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, username),
                    new Claim(ClaimTypes.Surname, username)
                }),
                Expires = DateTime.UtcNow.AddHours(5),
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(tokenKey),
                    SecurityAlgorithms.HmacSha256Signature
                )
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        public async Task<User> GetCurrentUserAsync(ClaimsIdentity claimsIdentity)
        {
            var username = claimsIdentity.FindFirst(ClaimTypes.Name)?.Value;
            return await _userRepository.GetUserByUsernameAsync(username);
        }
    }
}