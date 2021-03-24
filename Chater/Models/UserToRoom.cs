﻿using System;
using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Chater.Models
{
    public class UserToRoom
    {
        
        public const int Owner = 0;
        public const int Administration = 1;
        public const int SimpleUser = 2;
        
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public string User { get; set; }
        
        public string Room { get; set; }
        
        public int Roles { get; set; }
        
    }
}