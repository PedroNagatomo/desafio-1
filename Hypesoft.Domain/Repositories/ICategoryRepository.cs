using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hypesoft.Domain.Entities;

namespace Hypesoft.Domain.Repositories
{
    public interface ICategoryRepository : IBaseRepository<Category>
    {
        Task<(IEnumerable<Category> Categories, long TotalCount)> GetPagedAsync(
            int page,
            int pageSize,
            string? searchTerm = null,
            bool? isActive = null,
            CancellationToken cancellationToken = default
        );

        Task<IEnumerable<Category>> GetActiveCategoriesAsync(
            CancellationToken cancellationToken = default
        );

        Task<bool> ExistsByNameAsync(
            string name,
            string? excludeId = null,
            CancellationToken cancellationToken = default
        );

        Task<bool> HasProductsAsync(
            string categoryId,
            CancellationToken cancellationToken = default
        );

        Task<IEnumerable<Category>> SearhByNameAsync(
            string searchTerm,
            CancellationToken cancellationToken = default
        );

        Task<Category?> GetByNameAsync(
            string name,
            CancellationToken cancellationToken = default
        );
    }
}