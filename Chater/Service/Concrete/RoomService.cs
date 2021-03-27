using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Chater.Dtos.Room.From;
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
        private readonly IRoomServiceHelper _roomServiceHelper;

        public RoomService(IRoomRepository roomRepository, IUserToRoomRepository userToRoomRepository, IRoomServiceHelper roomServiceHelper)
        {
            _roomRepository = roomRepository;
            _userToRoomRepository = userToRoomRepository;
            _roomServiceHelper = roomServiceHelper;
        }

        public async Task<RoomAction> CreateRoomAsync(CreateUpdateRoomDto createRoom, User user)
        {
            if (await _roomServiceHelper.RoomIsExistAsync(createRoom.Name))
            {
                throw new RoomWithThisNameExist("Room with this name exist");
            }
            Room newRoom = new()
            {
                Name = createRoom.Name,
                Password = createRoom.Password is null ? null : BCrypt.Net.BCrypt.HashPassword(createRoom.Password),
                Chats = null
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
        
        
        public async Task<RoomAction> UpdateRoomAsync(CreateUpdateRoomDto updateRoom, User user)
        {
            Room room = await _roomRepository.GetRoomByNameAsync(updateRoom.Name);
            await _roomServiceHelper.VerificationDataBeforeUpdate(updateRoom, user);
            room.Name = updateRoom.Name;
            await _roomRepository.UpdateRoomAsync(room); 
            
            return new RoomAction()
            {
                IsSuccessfully = true,
                Room = room.asDto()
            };
        }

        

        public async Task AddUserToRoomAsync(User user, Room room, int role, string password = null)
        {
            await _roomServiceHelper.VerificationDataBeforeAddUserToRoomAsync(user, room, role, password);
            await CreateUserToRoomAndAddAsync(user.Id, room.Id, role);
        }

        public Task<RoomAction> RemoveUserFromRoomAsync(User user, Room room, string password = null)
        {
            throw new System.NotImplementedException();
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