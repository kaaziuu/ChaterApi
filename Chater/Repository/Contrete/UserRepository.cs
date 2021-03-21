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

        public async Task<User> GetUserAsync(string id)
        {
            var filter = _filterDefinitionBuilder.Eq(u => u.Id, id);
            return await _collection.Find(filter).SingleOrDefaultAsync();
        }

        public async Task CreateUserAsync(User user)
        {
            await _collection.InsertOneAsync(user); 
        }

        public async Task UpdateUserAsync(User user)
        {
            var filter = _filterDefinitionBuilder.Eq(eu => eu.Id, user.Id);
            await _collection.ReplaceOneAsync(filter, user);
        }

        public async Task DeleteUserAsync(string id)
        {
            var filter = _filterDefinitionBuilder.Eq(user => user.Id, id);
            await _collection.DeleteOneAsync(filter);
        }

        public async Task<User> GetUserByUsernameAsync(string username)
        {
            var user = ( await _collection.Find(x => x.Username == username).FirstOrDefaultAsync());
            return user;
        }
    }
}