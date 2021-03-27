using System.Collections.Generic;
using System.Threading.Tasks;
using Chater.Dtos.Room.From;
using Chater.Dtos.Room.Response;
using Chater.Models;
using Chater.Service.Concrete;

namespace Chater.Service.Abstract
{
    public interface IRoomService
    {
        Task<RoomAction> CreateRoomAsync(CreateUpdateRoomDto createRoom, User user);

        Task<RoomAction> UpdateRoomAsync(CreateUpdateRoomDto updateRoom, User user);
        
        Task AddUserToRoomAsync(User user, Room room, int role, string password = null);

        Task<RoomAction> RemoveUserFromRoomAsync(User user, Room room, string password = null);
        
    }
}