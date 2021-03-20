using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Policy;
using System.Threading.Tasks;
using Chater.Dtos.User;
using Chater.Models;
using Chater.Repository.Abstract;
using Chater.Repository.Contrete;
using Chater.Settings;
using Microsoft.AspNetCore.Mvc;

namespace Chater.Controllers
{
    [ApiController]
    [Route("users")]
    public class UserController : ControllerBase
    {
        private IUserRepository _userRepository;

        public UserController(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        [HttpGet]
        public async Task<IEnumerable<UserDto>> GetUsers()
        {
            var users = (await _userRepository.GetUsersAsync()).Select(u => u.AsDto());
            return users;
        }


        [HttpPost]
        public async Task<ActionResult> CreateUserAsync(CreateUserDto dto)
        {
            if ( await _userRepository.UserExist(dto.Username))
            {
                return BadRequest();
            }

            User user = new User()
            {
                Username = dto.Username,
                Name =  dto.Name,
                Surname = dto.Surname,
                Password = BCrypt.Net.BCrypt.HashPassword(dto.Password),
            };
            _userRepository.CreateUserAsync(user);

            return NoContent();


        }
    }
}