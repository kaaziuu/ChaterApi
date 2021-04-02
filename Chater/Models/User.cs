using System;
using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Chater.Models
{
    public class User
    {
        
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        
        public string ExtendId { get; set; }
        
        public string Username { get; set; }
        
        public string Name { get; set; }

        public string Surname { get; set; }
        
        [BsonElement("Password")]
        public string Password { get; set; }
        
        

    }
}