using Chater.Dtos.Room.Response;
using Chater.Models;

namespace Chater.Extensions
{
    public static class RoomExtensions
    {
        public static RoomDto asDto(this Room room) => new RoomDto()
        {
            Chats = room.Chats,
            Id = room.Id,
            Name = room.Name,
        };
    }
}