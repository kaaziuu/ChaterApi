using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Policy;
using System.Threading.Tasks;
using Chater.Dtos.User.From;
using Chater.Models;
using Chater.Repository.Abstract;
using Chater.Repository.Contrete;
using Chater.Service.Abstract;
using Chater.Settings;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Chater.Dtos.User.Response;

namespace Chater.Controllers
{
    [Authorize]
    [ApiController]
    [Route("users")]
    public class UserController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        private readonly IIdentityService _identityService;


        public UserController(IUserRepository userRepository, IIdentityService identityService)
        {
            _userRepository = userRepository;
            _identityService = identityService;
        }


        [AllowAnonymous]
        [HttpPost]
        public async Task<ActionResult<AuthDto>> CreateUserAsync(CreateUserDto request)
        {
            if ((await _userRepository.GetUserByUsernameAsync(request.Username)) is not null)
            {
                return BadRequest();
            }

            var newUser = new User()
            {
                Username = request.Username,
                Name = request.Name,
                Surname = request.Surname,
                Password = BCrypt.Net.BCrypt.HashPassword(request.Password)
            };

            await _userRepository.CreateUserAsync(newUser);

            return new AuthDto()
            {
                Token = await _identityService.AuthenticateAsync(request.Username, request.Password)
            };
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<ActionResult<AuthDto>> LoginAsync(LoginDto request)
        {
            var token = await _identityService.AuthenticateAsync(request.Username, request.Password);
            if (token is null)
                return BadRequest();
            return new AuthDto()
            {
                Token = token
            };
        }
    }
}