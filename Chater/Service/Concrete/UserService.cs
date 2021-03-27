using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Chater.Dtos.Room.Response;
using Chater.Extensions;
using Chater.Models;
using Chater.Repository.Abstract;
using Chater.Service.Abstract;
using MongoDB.Driver.Core.Connections;

namespace Chater.Service.Concrete
{
    public class UserService : IUserService
    {
        private readonly IUserToRoomRepository _userToRoomRepository;
        private readonly IRoomRepository _roomRepository;
        public UserService(IUserToRoomRepository userToRoomRepository, IRoomRepository roomRepository)
        {
            _userToRoomRepository = userToRoomRepository;
            _roomRepository = roomRepository;
        }
        
        public async Task<ICollection<RoomDto>> GetUserRoomsAsync(User user)
        {
            ICollection<UserToRoom> userToRooms = await _userToRoomRepository.GetUserRoomAsync(user);
            ICollection<RoomDto> userRooms = new List<RoomDto>();
            foreach (var userToRoom in userToRooms)
            {
                var room = await _roomRepository.GetRoomAsync(userToRoom.Room);
                userRooms.Add(room.asDto());
            }

            return userRooms;

        }
    }
}