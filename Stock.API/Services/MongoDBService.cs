using MongoDB.Driver;

namespace Stock.API.Services
{
    public class MongoDBService
    {
        readonly IMongoDatabase _mongoDb;

        public MongoDBService(IConfiguration configuration)
        {
            MongoClient client = new(configuration.GetConnectionString("MongoDB"));

            _mongoDb = client.GetDatabase("StockAPIDB");
        }

        // Hangi entity' veriyorsak ona göre collection'ı çekecek.
        public IMongoCollection<T> GetCollection<T>() => _mongoDb.GetCollection<T>(typeof(T).Name.ToLowerInvariant());
    }
}
