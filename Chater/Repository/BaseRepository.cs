using MongoDB.Driver;

namespace Chater.Repository
{
    public class BaseRepository
    {
        private const string DatabaseName = "Chat";
        protected IMongoDatabase Database;

        public BaseRepository(IMongoClient mongoClient)
        {
            Database = mongoClient.GetDatabase(DatabaseName);
        }
    }
}