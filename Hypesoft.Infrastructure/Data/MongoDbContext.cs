using Hypesoft.Domain.Entities;
using Hypesoft.Infrastructure.Configuration;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;

namespace Hypesoft.Infrastructure.Data
{
    public class MongoDbContext
    {
        private readonly IMongoDatabase _database;

        public MongoDbContext(IConfiguration configuration)
        {
            var settings = configuration.GetSection("MongoDbSettings").Get<MongoDbSettings>()
                ?? throw new InvalidOperationException("MongoDbSettings not configured");

            var client = new MongoClient(settings.ConnectionString);
            _database = client.GetDatabase(settings.DatabaseName);
        }

        public IMongoCollection<Product> Products => _database.GetCollection<Product>("Products");
        public IMongoCollection<Category> Categories => _database.GetCollection<Category>("Categorias");

        public async Task CreateIndexesAsync()
        {
            var productIndexes = new[]
            {
                new CreateIndexModel<Product>(
                    Builders<Product>.IndexKeys.Ascending(p => p.Name),
                    new CreateIndexOptions {Unique = true}),

                new CreateIndexModel<Product>(
                    Builders<Product>.IndexKeys.Ascending(p => p.CategoryId)),

                new CreateIndexModel<Product>(
                    Builders<Product>.IndexKeys.Ascending(p => p.SKU),
                    new CreateIndexOptions{
                        Unique = true,
                        Sparse = true
                    }),

                new CreateIndexModel<Product>(
                    Builders<Product>.IndexKeys.Ascending(p => p.IsActive)),

                new CreateIndexModel<Product>(
                    Builders<Product>.IndexKeys.Descending(p => p.createdAt)),

                new CreateIndexModel<Product>(
                    Builders<Product>.IndexKeys.Text(p => p.Name).Text(p => p.Description))
            };

            await Products.Indexes.CreateManyAsync(productIndexes);

            var categoryIndexes = new[]
            {
                new CreateIndexModel<Category>(
                    Builders<Category>.IndexKeys.Ascending(c => c.Name),
                    new CreateIndexOptions {Unique = true}),

                new CreateIndexModel<Category>(
                    Builders<Category>.IndexKeys.Ascending(c => c.IsActive)),

                new CreateIndexModel<Category>(
                    Builders<Category>.IndexKeys.Text(c => c.Name).Text(c => c.Description))
            };

            await Categories.Indexes.CreateManyAsync(categoryIndexes);
        }
    }
}