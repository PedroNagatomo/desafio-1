using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hypesoft.Domain.Entities;
using Hypesoft.Domain.Repositories;
using Hypesoft.Infrastructure.Data;
using MongoDB.Driver;

namespace Hypesoft.Infrastructure.Repositories;

public class CategoryRepository : BaseRepository<Category>, ICategoryRepository
{
    private readonly IProductRepository _productRepository;

    public CategoryRepository(MongoDbContext context, IProductRepository productRepository)
        : base(context.Categories)
    {
        _productRepository = productRepository;
    }

    public async Task<(IEnumerable<Category> Categories, long TotalCount)> GetPagedAsync(
        int page, 
        int pageSize, 
        string? searchTerm = null,
        bool? isActive = null,
        CancellationToken cancellationToken = default)
    {
        var filterBuilder = Builders<Category>.Filter;
        var filters = new List<FilterDefinition<Category>>();

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            filters.Add(filterBuilder.Text(searchTerm));
        }

        if (isActive.HasValue)
        {
            filters.Add(filterBuilder.Eq(c => c.IsActive, isActive.Value));
        }

        var filter = filters.Any() 
            ? filterBuilder.And(filters)
            : FilterDefinition<Category>.Empty;

        var query = _collection.Find(filter).SortBy(c => c.Name);
        
        var totalCount = await query.CountDocumentsAsync(cancellationToken);
        
        var categories = await query
            .Skip((page - 1) * pageSize)
            .Limit(pageSize)
            .ToListAsync(cancellationToken);

        return (categories, totalCount);
    }

    public async Task<IEnumerable<Category>> GetActiveCategoriesAsync(CancellationToken cancellationToken = default)
    {
        return await _collection
            .Find(c => c.IsActive)
            .SortBy(c => c.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> ExistsByNameAsync(string name, string? excludeId = null, CancellationToken cancellationToken = default)
    {
        var filter = Builders<Category>.Filter.Eq(c => c.Name, name);
        
        if (!string.IsNullOrWhiteSpace(excludeId))
        {
            var excludeFilter = Builders<Category>.Filter.Ne(c => c.Id, excludeId);
            filter = Builders<Category>.Filter.And(filter, excludeFilter);
        }

        return await _collection.Find(filter).AnyAsync(cancellationToken);
    }

    public async Task<bool> HasProductsAsync(string categoryId, CancellationToken cancellationToken = default)
    {
        return await _productRepository.ExistsAsync(p => p.CategoryId == categoryId, cancellationToken);
    }

    public async Task<IEnumerable<Category>> SearchByNameAsync(string searchTerm, CancellationToken cancellationToken = default)
    {
        var filter = Builders<Category>.Filter.Text(searchTerm);
        return await _collection.Find(filter).ToListAsync(cancellationToken);
    }

    public async Task<Category?> GetByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        return await _collection
            .Find(c => c.Name == name)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public Task<IEnumerable<Category>> SearhByNameAsync(string searchTerm, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}
