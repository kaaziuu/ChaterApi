using System;
using MongoDB.Bson;

namespace Chater.Dtos.User.Response
{
    public record UserDto
    {
        public string Id { get; set; }
        
        public string Username { get; set; }
        
        public string Name { get; set; }
        
        public string Surname { get; set; }
    }
}