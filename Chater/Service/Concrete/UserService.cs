using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Chater.Models;
using Chater.Repository.Abstract;
using Chater.Service.Abstract;
using MongoDB.Driver.Core.Connections;

namespace Chater.Service.Concrete
{
    public class UserService : IUserService
    {
        private readonly IUserToRoomRepository _userToRoomRepository;

        public UserService(IUserToRoomRepository userToRoomRepository)
        {
            _userToRoomRepository = userToRoomRepository;
        }
        public async Task<ICollection<Room>> GetUserRoomsAsync(User user)
        {
            ICollection<UserToRoom> userToRooms = await _userToRoomRepository.GetUserRoomAsync(user);
            ICollection<Room> userRooms = new List<Room>();
            foreach (var userToRoom in userToRooms)
            {
                userRooms.Add(userToRoom.Room);
            }

            return userRooms;

        }
    }
}