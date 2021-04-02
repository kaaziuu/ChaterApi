using System.Collections.Generic;
using System.Threading.Tasks;
using Chater.Models;

namespace Chater.Repository.Abstract
{
    public interface IRoomRepository
    {

        Task<Room> GetRoomAsync(string id);

        Task<Room> GetRoomByNameAsync(string name);

        Task CreateRoomAsync(Room room);

        Task UpdateRoomAsync(Room room);

        Task DeleteRoomAsync(Room room);




    }
}