using System.Collections.Generic;
using Chater.Models;

namespace Chater.Dtos.Room.Response
{
    public record RoomDto
    {
        public string Id { get; set; }
        
        public string Name { get; set; }
        
        public IEnumerable<string> Chats { get; set; }
    }
}