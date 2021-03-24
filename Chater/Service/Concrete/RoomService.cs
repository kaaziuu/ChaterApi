using System;
using System.Threading.Tasks;
using Chater.Dtos.Room.From;
using Chater.Dtos.Room.Response;
using Chater.Models;
using Chater.Repository.Abstract;
using Chater.Service.Abstract;

namespace Chater.Service.Concrete
{
    public class RoomService : IRoomService
    {
        private readonly IRoomRepository _roomRepository;
        private readonly IUserToRoomRepository _userToRoomRepository;

        public RoomService(IRoomRepository roomRepository, IUserToRoomRepository userToRoomRepository)
        {
            _roomRepository = roomRepository;
            _userToRoomRepository = userToRoomRepository;
        }

        public async Task<RoomAction> CreateRoomAsync(CreateUpdateRoomDto createRoom, User user)
        {
            var existRoom = await _roomRepository.GetRoomByNameAsync(createRoom.Name);
            if (existRoom is not null)
            {
                return new RoomAction()
                {
                    Error = "Room with this name exist",
                    IsSuccessfully = false,
                    Room = null
                };
            }
            Room newRoom = new()
            {
                Name = createRoom.Name,
                Password = createRoom.Password is null ? null : BCrypt.Net.BCrypt.HashPassword(createRoom.Password),
                Chats = null
            };

            UserToRoom userToRoom = new()
            {
                Room = newRoom,
                Roles = UserToRoom.Owner,
                User = user
            };

            await _roomRepository.CreateRoomAsync(newRoom);
            await _userToRoomRepository.AddUserToRoomAsync(userToRoom);
            return new RoomAction()
            {
                IsSuccessfully = true,
                Room = newRoom,
                Error = null
            };

        }
        
        
        public Task<RoomAction> UpdateRoomAsync(CreateUpdateRoomDto updateRoom, User user)
        {
            throw new System.NotImplementedException();

        }

        public bool PasswordVerificationAsync(Room room, string password)
        {
            if (BCrypt.Net.BCrypt.Verify(password, room.Password))
                return true;
            return false;
        }

        public async Task<bool> PasswordVerificationByRoomNameAsync(string roomName, string password)
        {
            var room = await _roomRepository.GetRoomByNameAsync(roomName);
            if (PasswordVerificationAsync(room, password))
                return true;
            return false;
        }

        public Task<RoomAction> AddUserToRoomAsync(User user, Room room, int roles, string password = null)
        {
            throw new System.NotImplementedException();
        }

        public Task<RoomAction> RemoveUserFromRoomAsync(User user, Room room)
        {
            throw new System.NotImplementedException();
        }
        
    }
}