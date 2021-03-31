using System.Threading.Tasks;
using Chater.Dtos.Room;
using Chater.Dtos.Room.Form;
using Chater.Models;

namespace Chater.Service.Abstract.HelperService
{
    public interface IRoomServiceHelper
    {
        Task<bool> VerificationDataBeforeUpdate(UpdateRoomForm updateForm, User user);
        bool PasswordVerification(Room room, string password);

        Task<bool> PasswordVerificationByRoomNameAsync(string roomName, string password);

        Task<bool> RoomIsExistAsync(string roomName);

        Task<int?> GetUserRoleInRoomAsync(Room room, User user);

        Task VerificationDataBeforeAddUserToRoomAsync(User user, Room room, int role, string password = null);



    }
}