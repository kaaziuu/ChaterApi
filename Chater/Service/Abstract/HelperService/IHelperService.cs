using System.Threading.Tasks;
using Chater.Dtos.Room;
using Chater.Dtos.Room.Form;
using Chater.Models;

namespace Chater.Service.Abstract.HelperService
{
    public interface IHelperService
    {
        Task<bool> VerificationDataBeforeUpdate(UpdateRoomForm updateForm, User user);
        bool VerificationPassword(Room room, string password);

        Task<bool> PasswordVerificationByRoomNameAsync(string roomName, string password);

        Task<bool> RoomIsExistAsync(string roomName);

        Task<int?> GetUserRoleInRoomAsync(Room room, User user);

        Task VerificationDataBeforeAddUserToRoomAsync(User user, Room room, int role, string password = null);

        Task VerificationDataBeforeRemoveUserToRoomAsync(User user, RemoveUserForm form, Room? room, User userToRemove);

        Task VerificationDataBeforeSendMessage(User user, Room? room);


    }
}