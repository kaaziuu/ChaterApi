using System;
using Chater.Dtos.User.Response;
using Chater.Models;

namespace Chater.Extensions
{
    public static class UserExtensions
    {
        public static UserDto AsDto( this User  user)
        {
            return new UserDto()
            {
                Id =  user.Id,
                Username = user.Username,
                Name = user.Name,
                Surname = user.Surname
            };
        }

        public static User AsUser(this UserDto dto)
        {
            return new User()
            {
                Id = dto.Id,
                Username = dto.Username,
                Name = dto.Name,
                Surname = dto.Surname
            };
        }
    }
}