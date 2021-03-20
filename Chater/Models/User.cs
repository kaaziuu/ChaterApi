using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Chater.Models
{
    public class User
    {
        public ObjectId Id { get; set; }
        
        public string Username { get; set; }
        
        public string Name { get; set; }

        public string Surname { get; set; }

        public string Password { get; set; }
        
        
        public string Token { get; set; }
         
    }
}