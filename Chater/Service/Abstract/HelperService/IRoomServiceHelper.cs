using System.Threading.Tasks;
using Chater.Dtos.Room.From;
using Chater.Models;

namespace Chater.Service.Abstract.HelperService
{
    public interface IRoomServiceHelper
    {
        Task<bool> VerificationDataBeforeUpdate(CreateUpdateRoomDto updateForm, User user);
        bool PasswordVerification(Room room, string password);

        Task<bool> PasswordVerificationByRoomNameAsync(string roomName, string password);

        Task<bool> RoomIsExistAsync(string roomName);

        Task<int?> GetUserRoleAsync(Room room, User user);

        Task VerificationDataBeforeAddUserToRoomAsync(User user, Room room, int role, string password = null);

    }
}