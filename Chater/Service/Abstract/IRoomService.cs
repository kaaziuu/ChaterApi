using System.Collections.Generic;
using System.Threading.Tasks;
using Chater.Dtos.Room;
using Chater.Dtos.Room.Form;
using Chater.Dtos.Room.Response;
using Chater.Models;
using Chater.Service.Concrete;

namespace Chater.Service.Abstract
{
    public interface IRoomService
    {
        Task<RoomAction> CreateRoomAsync(CreateRoomForm createRoom, User user);

        Task<RoomAction> UpdateRoomAsync(UpdateRoomForm updateRoom, User user);

        Task GetRoomAndAddUserAsync(AddRemoveUserFromRoom form, string roomId);

        Task RemoveUserFromRoomAsync(User user, Room room, string password = null);
        
        
    }
}