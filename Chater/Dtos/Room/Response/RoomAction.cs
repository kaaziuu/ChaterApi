using System.Collections.Generic;

namespace Chater.Dtos.Room.Response
{
    public record RoomAction
    {
        public bool IsSuccessfully { get; set; }
        public IEnumerable<string> Error { get; set; }
    }
}