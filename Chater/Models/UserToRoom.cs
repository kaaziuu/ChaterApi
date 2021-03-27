using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Chater.Models
{
    public class UserToRoom
    {
        
        public const int Owner = 0;
        public const int Administration = 1;
        public const int SimpleUser = 2;

        public static List<T> GetAllRoles<T>(Type type)
        {
             FieldInfo[] fields = type.GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy);
             return (fields.Where(fl => fl.IsLiteral && fl.FieldType == typeof(T))
                 .Select(fi => (T) fi.GetRawConstantValue())).ToList();
        }
        
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public string User { get; set; }
        
        public string Room { get; set; }
        
        public int Roles { get; set; }
        
    }
}