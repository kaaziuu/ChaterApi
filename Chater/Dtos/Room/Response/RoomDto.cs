using System.Collections.Generic;
using Chater.Models;

namespace Chater.Dtos.Room.Response
{
    public record RoomDto
    {
        public string Id { get; set; }
        
        public string Name { get; set; }
        
        public IEnumerable<UserToRoom> Users { get; set; }
        
        public IEnumerable<Chat> Chats { get; set; }
    }
}