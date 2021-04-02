using System;
using MongoDB.Bson.Serialization.Attributes;

namespace Chater.Models
{
    public class Message
    {
        [BsonElement]
        public string UserId { get; set; }
        [BsonElement]
        public string Text { get; set; }
        [BsonElement]
        public DateTime SendAt { get; set; }
    }
}