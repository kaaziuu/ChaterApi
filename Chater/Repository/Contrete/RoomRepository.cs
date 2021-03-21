using System.Collections.Generic;
using System.Threading.Tasks;
using Chater.Models;
using Chater.Repository.Abstract;
using MongoDB.Driver;

namespace Chater.Repository.Contrete
{
    public class RoomRepository:BaseRepository, IRoomRepository
    {
        public RoomRepository(IMongoClient mongoClient) : base(mongoClient)
        {
        }

        public Task<Room> GetUserRoomsAsync(string room)
        {
            throw new System.NotImplementedException();
        }

        public Task<Room> GetRoomAsync(string id)
        {
            throw new System.NotImplementedException();
        }

        public Task<Room> GetRoomByNameAsync(string name)
        {
            throw new System.NotImplementedException();
        }

        public Task CreateRoomAsync(Room room)
        {
            throw new System.NotImplementedException();
        }

        public Task UpdateRoomAsync(Room room)
        {
            throw new System.NotImplementedException();
        }

        public Task DeleteRoomAsync(Room room)
        {
            throw new System.NotImplementedException();
        }

        public Task DeleteUserAsync(User user)
        {
            throw new System.NotImplementedException();
        }

        public Task<IEnumerable<Room>> GetUserRoomsAsync(User user)
        {
            throw new System.NotImplementedException();
        }
    }
}