using System.Collections.Generic;
using MongoDB.Bson.Serialization.Attributes;

namespace Chater.Models
{
    public class UserToRoom
    {
        
        public const int Owner = 0;
        public const int Administration = 1;
        public const int SimpleUser = 2;
        
        public User User { get; set; }
        
        public Room Room { get; set; }
        
        public int Roles { get; set; }
        
    }
}