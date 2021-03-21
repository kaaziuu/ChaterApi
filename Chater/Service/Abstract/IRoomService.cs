using System.Collections.Generic;
using System.Threading.Tasks;
using Chater.Dtos.Room.From;
using Chater.Dtos.Room.Response;
using Chater.Models;

namespace Chater.Service.Abstract
{
    public interface IRoomService
    {
        Task<RoomAction> CreateRoomAsync(CreateUpdateRoomDto createRoom);

        Task<RoomAction> UpdateRoomAsync(CreateUpdateRoomDto updateRoom);
        
        Task<RoomAction> AddUserToRoomAsync(User user, Room room,string password = null);

        Task<RoomAction> RemoveUserFromRoomAsync(User user, Room room);
    }
}