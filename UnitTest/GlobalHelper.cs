using System;
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
                Room = room,
                User = user
            };
        }
        
        public static Room GenerateRoom()
        {
            return new Room()
            {
                Chats = null,
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
    }
}