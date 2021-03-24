﻿using System;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using Chater.Dtos.User.Response;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Chater.Models
{
    public class Chat
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        
        [BsonElement]
        public ICollection<Message> Messages { get; set; }
        
        [BsonElement]
        public ICollection<UserDto> Users { get; set; }
        
        
    }
}