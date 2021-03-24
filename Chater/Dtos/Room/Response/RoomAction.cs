using System.Collections.Generic;

namespace Chater.Dtos.Room.Response
{
    public record RoomAction
    {
        public bool IsSuccessfully { get; set; }
        public string Error { get; set; }
        
        public Models.Room Room { get; set; }
    }
}