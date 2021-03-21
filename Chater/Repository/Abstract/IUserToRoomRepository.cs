using System.Collections.Generic;
using System.Threading.Tasks;
using Chater.Models;

namespace Chater.Repository.Abstract
{
    public interface IUserToRoomRepository
    {
        Task<ICollection<UserToRoom>> GetUserRoomAsync(User user);

        Task DeleteUserFromRoomAsync(User user, Room room);
    }
}