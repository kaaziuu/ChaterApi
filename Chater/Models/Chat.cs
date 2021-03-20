using System;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Chater.Models
{
    public class Chat
    {
        public Guid Id { get; set; }
        
        [BsonExtraElements]
        public IEnumerable<User> Users { get; set; }
    }
}