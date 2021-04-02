using System;
using System.Security.Claims;
using Chater.Models;

namespace UnitTest
{
    public class GlobalHelper
    {
        private static Random _random = new Random();
        
        public static UserToRoom AssignUserToRoom(User user, Room room, int roles=-1)
        {
            return new UserToRoom()
            {
                Roles = (roles != -1) ? roles : _random.Next(UserToRoom.Owner, UserToRoom.SimpleUser),
                Room = room.Id,
                User = user.Id
            };
        }
        
        public static Room GenerateRoom()
        {
            return new Room()
            {
                Messages = null,
                Id = new Guid().ToString(),
                Name = new Guid().ToString(),
                Password = new Guid().ToString()
            };
        }

        public static User GenerateExampleUser()
        {
            return new User()
            {
                Id = new Guid().ToString(),
                Name = new Guid().ToString(),
                Password = new Guid().ToString(),
                Surname = new Guid().ToString(),
                Username = new Guid().ToString()
            };
        }

        public static ClaimsPrincipal FakeAuthenticationUser()
        {
            return new ClaimsPrincipal(new ClaimsIdentity(new Claim[] {
                new Claim(ClaimTypes.NameIdentifier, "admin"),
                new Claim(ClaimTypes.Name, "admin")
                // other required and custom claims
            },"TestAuthentication"));
        }
    }
}