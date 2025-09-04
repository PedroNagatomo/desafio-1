using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Hypesoft.Domain.Entities;

namespace Hypesoft.Domain.Repositories
{
    public interface IProductRepository : IBaseRepository<Product>
    {
        Task<(IEnumerable<Product> Products, long TotalCount)> GetPagedAsync(
            int page,
            int pageSize,
            Expression<Func<Product, bool>>? filter = null,
            Expression<Func<Product, object>>? orderBy = null,
            bool ascending = true,
            CancellationToken cancellationToken = default);

        Task<IEnumerable<Product>> SearchByNameAsync(
            string searchTerm,
            CancellationToken cancellationToken = default);

        Task<IEnumerable<Product>> GetByCategoryIdAsync(
            string categoryId,
            CancellationToken cancellationToken = default);

        Task<IEnumerable<Product>> GetLowStockProductsAsync(
            int threshold = 10,
            CancellationToken cancellationToken = default);

        Task<IEnumerable<Product>> GetActiveProductsAsync(
            CancellationToken cancellationToken = default);

        Task<bool> ExistsByNameAsync(
            string name,
            string? excludeId = null,
            CancellationToken cancellationToken = default);

        Task<bool> ExistsBySKUAsync(
            string sku,
            string? excludeId = null,
            CancellationToken cancellationToken = default);

        Task<decimal> GetTotalStockValueAsync(
            CancellationToken cancellationToken = default);

        Task<Dictionary<string, int>> GetProductCountByCategoryAsync(
            CancellationToken cancellationToken = default);

        Task<IEnumerable<Product>> GetMostRecentAsync(
            int count = 10,
            CancellationToken cancellationToken = default);

    }
}