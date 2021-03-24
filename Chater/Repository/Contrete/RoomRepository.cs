using System.Collections.Generic;
using System.Threading.Tasks;
using Chater.Models;
using Chater.Repository.Abstract;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Chater.Repository.Contrete
{
    public class RoomRepository:BaseRepository, IRoomRepository
    {
        
        private readonly IMongoCollection<Room> _collection;
        private readonly FilterDefinitionBuilder<Room> _filterDefinitionBuilder = Builders<Room>.Filter;
        public RoomRepository(IMongoClient mongoClient) : base(mongoClient)
        {
            _collection = Database.GetCollection<Room>(nameof(Room));
        }

        public async Task<Room> GetRoomAsync(string id)
        {
            var filter = _filterDefinitionBuilder.Eq(r => r.Id, id);
            return await _collection.Find(filter).SingleOrDefaultAsync();
        }

        public async Task<Room> GetRoomByNameAsync(string name)
        {
            var filter = _filterDefinitionBuilder.Eq(r => r.Name, name);
            return await _collection.Find(filter).SingleOrDefaultAsync();
        }

        public async Task CreateRoomAsync(Room room)
        {
            _collection.InsertOneAsync(room);
        }

        public async Task UpdateRoomAsync(Room room)
        {
            var filter = _filterDefinitionBuilder.Eq(r => r.Id, room.Id);
            await _collection.ReplaceOneAsync(filter, room);
        }

        public async Task DeleteRoomAsync(Room room)
        {
            var filter = _filterDefinitionBuilder.Eq(r => r.Id, room.Id);
            await _collection.DeleteOneAsync(filter);
        }




    }
}