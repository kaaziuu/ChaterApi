using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Chater.Models;
using Chater.Repository.Abstract;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Chater.Repository.Contrete
{
    public class UserRepository : BaseRepository, IUserRepository
    {

        private readonly IMongoCollection<User> _collection;
        private readonly FilterDefinitionBuilder<User> _filterDefinitionBuilder = Builders<User>.Filter;
        
        public UserRepository(IMongoClient mongoClient) : base(mongoClient)
        {
            _collection = Database.GetCollection<User>(nameof(User));
        }


        public async Task<IEnumerable<User>> GetUsersAsync()
        {
            return await _collection.Find(new BsonDocument()).ToListAsync();
        }

        public async Task<User> GetUserAsync(Guid id)
        {
            var filter = _filterDefinitionBuilder.Eq(u => u.UserId, id);
            return await _collection.Find(filter).SingleOrDefaultAsync();
        }

        public async Task CreateUserAsync(User user)
        {
            await _collection.InsertOneAsync(user); 
        }

        public async Task UpdateUserAsync(User user)
        {
            var filter = _filterDefinitionBuilder.Eq(eu => eu.UserId, user.UserId);
            await _collection.ReplaceOneAsync(filter, user);
        }

        public async Task DeleteUserAsync(Guid id)
        {
            var filter = _filterDefinitionBuilder.Eq(user => user.UserId, id);
            await _collection.DeleteOneAsync(filter);
        }

    }
}