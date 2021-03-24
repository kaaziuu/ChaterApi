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

        bool PasswordVerificationAsync(Room room, string password);

        Task<bool> PasswordVerificationByRoomNameAsync(string roomName, string password);

        Task<RoomAction> AddUserToRoomAsync(User user, Room room, int roles, string password = null);

        Task<RoomAction> RemoveUserFromRoomAsync(User user, Room room);
    }
}