using System;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Chater.Models
{
    public class Chat
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        
        [BsonExtraElements]
        public IEnumerable<Message> Messages { get; set; }
        
        [BsonElement]
        public IEnumerable<User> Users { get; set; }
        
        
    }
}