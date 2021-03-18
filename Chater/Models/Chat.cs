using System;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Chater.Models
{
    public class Chat
    {
        [BsonRepresentation(BsonType.String)]
        [BsonElement("chadId")]
        public Guid ChaiId { get; set; }
        
        [BsonExtraElements]
        public IEnumerable<User> Users { get; set; }
    }
}