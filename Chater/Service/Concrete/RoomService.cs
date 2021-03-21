using System.Threading.Tasks;
using Chater.Dtos.Room.From;
using Chater.Dtos.Room.Response;
using Chater.Models;
using Chater.Service.Abstract;

namespace Chater.Service.Concrete
{
    public class RoomService : IRoomService
    {
        public Task<RoomAction> CreateRoomAsync(CreateUpdateRoomDto createRoom)
        {
            throw new System.NotImplementedException();
        }

        public Task<RoomAction> UpdateRoomAsync(CreateUpdateRoomDto updateRoom)
        {
            throw new System.NotImplementedException();
        }

        public Task<RoomAction> AddUserToRoomAsync(User user, Room room, string password = null)
        {
            throw new System.NotImplementedException();
        }

        public Task<RoomAction> RemoveUserFromRoomAsync(User user, Room room)
        {
            throw new System.NotImplementedException();
        }
    }
}