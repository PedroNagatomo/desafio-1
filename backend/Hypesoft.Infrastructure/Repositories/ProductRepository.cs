using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Hypesoft.Domain.Entities;
using Hypesoft.Domain.Repositories;
using Hypesoft.Infrastructure.Data;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Hypesoft.Infrastructure.Repositories;

public class ProductRepository : BaseRepository<Product>, IProductRepository
{
    public ProductRepository(MongoDbContext context) : base(context.Products) { }

    public async Task<(IEnumerable<Product> Products, long TotalCount)> GetPagedAsync(
        int page,
        int pageSize,
        Expression<Func<Product, bool>>? filter = null,
        Expression<Func<Product, object>>? orderBy = null,
        bool ascending = true,
        CancellationToken cancellationToken = default)
    {
        var query = filter != null ? _collection.Find(filter) : _collection.Find(_ => true);

        if (orderBy != null)
        {
            query = ascending
                ? query.SortBy(orderBy)
                : query.SortByDescending(orderBy);
        }
        else
        {
            query = query.SortByDescending(p => p.createdAt);
        }

        var totalCount = await query.CountDocumentsAsync(cancellationToken);

        var products = await query
            .Skip((page - 1) * pageSize)
            .Limit(pageSize)
            .ToListAsync(cancellationToken);

        return (products, totalCount);
    }

    public async Task<IEnumerable<Product>> SearchByNameAsync(string searchTerm, CancellationToken cancellationToken = default)
    {
        var filter = Builders<Product>.Filter.Text(searchTerm);
        return await _collection.Find(filter).ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Product>> GetByCategoryIdAsync(string categoryId, CancellationToken cancellationToken = default)
    {
        return await FindAsync(p => p.CategoryId == categoryId, cancellationToken);
    }

    public async Task<IEnumerable<Product>> GetLowStockProductsAsync(int threshold = 10, CancellationToken cancellationToken = default)
    {
        return await _collection
            .Find(p => p.Stock.Value < threshold && p.IsActive)
            .SortBy(p => p.Stock.Value)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Product>> GetActiveProductsAsync(CancellationToken cancellationToken = default)
    {
        return await FindAsync(p => p.IsActive, cancellationToken);
    }

    public async Task<bool> ExistsByNameAsync(string name, string? excludeId = null, CancellationToken cancellationToken = default)
    {
        var filter = Builders<Product>.Filter.Eq(p => p.Name, name);
        
        if (!string.IsNullOrWhiteSpace(excludeId))
        {
            var excludeFilter = Builders<Product>.Filter.Ne(p => p.Id, excludeId);
            filter = Builders<Product>.Filter.And(filter, excludeFilter);
        }

        return await _collection.Find(filter).AnyAsync(cancellationToken);
    }

    public async Task<bool> ExistsBySKUAsync(string sku, string? excludeId = null, CancellationToken cancellationToken = default)
    {
        var filter = Builders<Product>.Filter.Eq(p => p.SKU, sku);
        
        if (!string.IsNullOrWhiteSpace(excludeId))
        {
            var excludeFilter = Builders<Product>.Filter.Ne(p => p.Id, excludeId);
            filter = Builders<Product>.Filter.And(filter, excludeFilter);
        }

        return await _collection.Find(filter).AnyAsync(cancellationToken);
    }

    public async Task<decimal> GetTotalStockValueAsync(CancellationToken cancellationToken = default)
    {
        var pipeline = new BsonDocument[]
        {
            new("$match", new BsonDocument("isActive", true)),
            new("$group", new BsonDocument
            {
                { "_id", BsonNull.Value },
                { "totalValue", new BsonDocument("$sum", 
                    new BsonDocument("$multiply", new BsonArray { "$price.amount", "$stock.value" })) }
            })
        };

        var result = await _collection.Aggregate<BsonDocument>(pipeline).FirstOrDefaultAsync(cancellationToken);
        return result?["totalValue"]?.AsDecimal ?? 0;
    }

    public async Task<Dictionary<string, int>> GetProductCountByCategoryAsync(CancellationToken cancellationToken = default)
    {
        var pipeline = new BsonDocument[]
        {
            new("$match", new BsonDocument("isActive", true)),
            new("$group", new BsonDocument
            {
                { "_id", "$categoryId" },
                { "count", new BsonDocument("$sum", 1) }
            })
        };

        var results = await _collection.Aggregate<BsonDocument>(pipeline).ToListAsync(cancellationToken);
        
        return results.ToDictionary(
            doc => doc["_id"].AsString,
            doc => doc["count"].AsInt32);
    }

    public async Task<IEnumerable<Product>> GetMostRecentAsync(int count = 10, CancellationToken cancellationToken = default)
    {
        return await _collection
            .Find(p => p.IsActive)
            .SortByDescending(p => p.createdAt)
            .Limit(count)
            .ToListAsync(cancellationToken);
    }
}