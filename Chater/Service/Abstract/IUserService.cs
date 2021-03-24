using System.Collections.Generic;
using System.Threading.Tasks;
using Chater.Dtos.Room.Response;
using Chater.Models;
using MongoDB.Driver.Core.Connections;

namespace Chater.Service.Abstract
{
    public interface IUserService
    {
        Task<ICollection<RoomDto>> GetUserRoomsAsync(User user);
    }
}