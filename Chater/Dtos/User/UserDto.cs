using System;
using MongoDB.Bson;

namespace Chater.Dtos.User
{
    public class UserDto
    {
        public ObjectId Id { get; set; }
        
        public string Username { get; set; }
        
        public string Name { get; set; }
        
        public string Surname { get; set; }
    }
}