using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Chater.Dtos.Room;
using Chater.Dtos.Room.Form;
using Chater.Exception;
using Chater.Models;
using Chater.Repository.Abstract;
using Chater.Repository.Contrete;
using Chater.Service.Abstract.HelperService;

namespace Chater.Service.Concrete.HelperService
{
    public class HelperService : IHelperService
    {
        private readonly IRoomRepository _roomRepository;
        private readonly IUserToRoomRepository _userToRoomRepository;

        
        public HelperService(IRoomRepository roomRepository, IUserToRoomRepository userToRoomRepository)
        {
            _roomRepository = roomRepository;
            _userToRoomRepository = userToRoomRepository;
        }
        
        public async Task<bool> VerificationDataBeforeUpdate(UpdateRoomForm updateForm, User user)
        {
            Room room = await _roomRepository.GetRoomByNameAsync(updateForm.Name);
            if ((await RoomIsExistAsync(updateForm.NewName)))
                throw new RoomWithThisNameExist("Room with this name exist");
            await VerificationRolesAsync(room, user, UserToRoom.Administration);
            return true;
        }

        public bool VerificationPassword(Room room, string password)
        {
            if (BCrypt.Net.BCrypt.Verify(password, room.Password))
                return true;
            throw new InvalidPasswordException("Invalid password");
        }

        public async Task<bool> PasswordVerificationByRoomNameAsync(string roomName, string password)
        {
            var room = await _roomRepository.GetRoomByNameAsync(roomName);
            if (VerificationPassword(room, password))
                return true;
            return false;
        }

        public async Task<bool> RoomIsExistAsync(string roomName)
        {
            var room = await _roomRepository.GetRoomByNameAsync(roomName);
            if (room is null)
                return false;
            return true;
        }

        public async Task<int?> GetUserRoleInRoomAsync(Room room, User user)
        {
            var userToRoom = await _userToRoomRepository.GetUserToRoomAsync(user, room);
            if (userToRoom is null)
                return null;
            return userToRoom.Roles;
        }

        public async Task VerificationDataBeforeAddUserToRoomAsync(User user, Room? room, int role,
            string password = null)
        {
            VerificationRoomExisting(room);
            VerificationPassword(room, password);
            VerificationRole(role);
            await VerificationUserIsInRoomAsync(user, room);
        }

        private async Task VerificationUserIsInRoomAsync(User user, Room room)
        {
            if (await _userToRoomRepository.UserIsOnRoomAsync(user, room))
            {
                throw new System.Exception("User in in room");
            }
        }
        
        public async Task VerificationDataBeforeRemoveUserToRoomAsync(User user, RemoveUserForm form, Room? room, User userToRemove)
        {
            if (room is null)
                throw new RoomDoesntExistExceptionException("room doesnt exist");
            await VerificationUserIsNotInRoomAsync(userToRemove, room);
            await VerificationRolesAsync(room, user, UserToRoom.Administration);
            VerificationPassword(room, form.Password);
        }
        
        private async Task VerificationRolesAsync(Room room, User user, int requireRole)
        {
            int? userRoles = await GetUserRoleInRoomAsync(room, user);
            if (!CheckRoles(userRoles, requireRole))
            {
                throw new InvalidRoleException("Invalid role");
            }
        }
        
        private bool CheckRoles(int? userRoles, int requireRoles)
        {
            if (userRoles is null)
                return false;
            if (userRoles <= requireRoles)
                return true;
            return false;
        }
        
        private void VerificationRole(int role)
        {
            List<int> roles = UserToRoom.GetAllRoles<int>(typeof(UserToRoom));
            if (!roles.Contains(role))
            {
                throw new InvalidRoleException("Invalid Role");
            }
        }
        

        public async Task VerificationDataBeforeSendMessage(User user, Room? room)
        {
            VerificationRoomExisting(room);
            await VerificationUserIsNotInRoomAsync(user, room);
        }

        private async Task VerificationUserIsNotInRoomAsync(User user, Room room)
        {
            if (!(await _userToRoomRepository.UserIsOnRoomAsync(user, room)))
            {
                throw new UserIsNotInRoomException("User is not in room");
            }
        }

        

        private void VerificationRoomExisting(Room room)
        {
            if (room == null)
            {
                throw new RoomDoesntExistExceptionException("Room doesnt exist");
            }
        }

        
    }
}