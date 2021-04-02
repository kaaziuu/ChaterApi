using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Chater.Dtos.Room;
using Chater.Dtos.Room.Form;
using Chater.Dtos.Room.Response;
using Chater.Exception;
using Chater.Extensions;
using Chater.Models;
using Chater.Repository.Abstract;
using Chater.Service.Abstract;
using Chater.Service.Abstract.HelperService;

namespace Chater.Service.Concrete
{
    public class RoomService : IRoomService
    {
        private readonly IRoomRepository _roomRepository;
        private readonly IUserToRoomRepository _userToRoomRepository;
        private readonly IHelperService _helperService;
        private readonly IUserRepository _userRepository;
        
        
        public RoomService(IRoomRepository roomRepository,
            IUserToRoomRepository userToRoomRepository,
            IHelperService helperService,
            IUserRepository userRepository)
        {
            _roomRepository = roomRepository;
            _userToRoomRepository = userToRoomRepository;
            _helperService = helperService;
            _userRepository = userRepository;
        }

        public async Task<RoomAction> CreateRoomAsync(CreateRoomForm createRoom, User user)
        {
            if (await _helperService.RoomIsExistAsync(createRoom.Name))
            {
                throw new RoomWithThisNameExist("Room with this name exist");
            }
            Room newRoom = new()
            {
                Name = createRoom.Name,
                Password = createRoom.Password is null ? null : BCrypt.Net.BCrypt.HashPassword(createRoom.Password),
                Messages = null
            };
            await _roomRepository.CreateRoomAsync(newRoom);
            await AddUserToRoomAsync(user, newRoom, UserToRoom.Administration, createRoom.Password);
            return new RoomAction()
            {
                IsSuccessfully = true,
                Room = newRoom.asDto(),
                Error = null
            };

        }
        
        public async Task<RoomAction> UpdateRoomAsync(UpdateRoomForm updateRoom, User user)
        {
            Room room = await _roomRepository.GetRoomAsync(updateRoom.Id);
            await _helperService.VerificationDataBeforeUpdate(updateRoom, user);
            room.Name = updateRoom.NewName;
            await _roomRepository.UpdateRoomAsync(room); 
            
            return new RoomAction()
            {
                IsSuccessfully = true,
                Room = room.asDto()
            };
        }

        public async Task GetRoomAndAddUserAsync(AddUserFromRoom form,  string roomId)
        {
            Room room = await _roomRepository.GetRoomAsync(roomId);
            User newUser = await _userRepository.GetUserAsync(form.UserId);
            await AddUserToRoomAsync(newUser, room, form.Role, form.RoomPassword);

        }
        
        private async Task AddUserToRoomAsync(User user, Room? room, int role, string password = null)
        {
            await _helperService.VerificationDataBeforeAddUserToRoomAsync(user, room, role, password);
            await CreateUserToRoomAndAddAsync(user.Id, room.Id, role);
        }

        public async Task RemoveUserFromRoomAsync(User user, RemoveUserForm form, string roomId)
        {
            User? userToRemove = await _userRepository.GetUserAsync(form.UserId);
            Room? room = await _roomRepository.GetRoomAsync(roomId);
            await _helperService.VerificationDataBeforeRemoveUserToRoomAsync(user, form, room, userToRemove);
            await _userToRoomRepository.DeleteUserFromRoomAsync(user, room);
        }

        private async Task CreateUserToRoomAndAddAsync(string userId, string roomId, int role)
        {
            UserToRoom userToRoom = new()
            {
                User = userId,
                Room = roomId,
                Roles = role
            };
            await _userToRoomRepository.AddUserToRoomAsync(userToRoom); 
        }
        
        
    }
}