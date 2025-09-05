using AutoMapper;
using MediatR;
using Hypesoft.Application.Queries.Dashboard;
using Hypesoft.Application.DTOs;
using Hypesoft.Domain.Repositories;

namespace Hypesoft.Application.Handlers.Dashboard;

public class GetDashboardHandler : IRequestHandler<GetDashboardQuery, DashboardDto>
{
    private readonly IProductRepository _productRepository;
    private readonly ICategoryRepository _categoryRepository;
    private readonly IMapper _mapper;

    public GetDashboardHandler(
        IProductRepository productRepository,
        ICategoryRepository categoryRepository,
        IMapper mapper)
    {
        _productRepository = productRepository;
        _categoryRepository = categoryRepository;
        _mapper = mapper;
    }

    public async Task<DashboardDto> Handle(GetDashboardQuery request, CancellationToken cancellationToken)
    {
        var tasks = new Task[]
        {
            _productRepository.CountAsync(cancellationToken: cancellationToken),
            _categoryRepository.CountAsync(cancellationToken: cancellationToken),
            _productRepository.GetLowStockProductsAsync(request.LowStockThreshold, cancellationToken),
            _productRepository.GetTotalStockValueAsync(cancellationToken),
            _productRepository.CountAsync(p => p.IsActive, cancellationToken),
            _productRepository.CountAsync(p => !p.IsActive, cancellationToken),
            _productRepository.GetProductCountByCategoryAsync(cancellationToken),
            _productRepository.GetMostRecentAsync(request.RecentProductsCount, cancellationToken)
        };

        await Task.WhenAll(tasks);

        var totalProducts = (int)(tasks[0] as Task<long>)!.Result;
        var totalCategories = (int)(tasks[1] as Task<long>)!.Result;
        var lowStockProducts = (tasks[2] as Task<IEnumerable<Domain.Entities.Product>>)!.Result;
        var totalStockValue = (tasks[3] as Task<decimal>)!.Result;
        var activeProducts = (int)(tasks[4] as Task<long>)!.Result;
        var inactiveProducts = (int)(tasks[5] as Task<long>)!.Result;
        var categoryProductCounts = (tasks[6] as Task<Dictionary<string, int>>)!.Result;
        var recentProducts = (tasks[7] as Task<IEnumerable<Domain.Entities.Product>>)!.Result;

        var categories = await _categoryRepository.GetAllAsync(cancellationToken);
        var categoryLookup = categories.ToDictionary(c => c.Id, c => c.Name);

        var stats = new DashboardStatsDto
        {
            TotalProducts = totalProducts,
            TotalCategories = totalCategories,
            LowStockProducts = lowStockProducts.Count(),
            TotalStockValue = totalStockValue,
            FormattedTotalStockValue = $"R$ {totalStockValue:N2}",
            ActiveProducts = activeProducts,
            InactiveProducts = inactiveProducts
        };

        var lowStockProductDtos = lowStockProducts.Select(p =>
        {
            var dto = _mapper.Map<ProductDto>(p);
            dto.CategoryName = categoryLookup.GetValueOrDefault(p.CategoryId, "Unknown Category");
            return dto;
        });

        var categoryStats = categoryProductCounts.Select(kvp => new CategoryStatsDto
        {
            CategoryId = kvp.Key,
            CategoryName = categoryLookup.GetValueOrDefault(kvp.Key, "Unknown Category"),
            ProductCount = kvp.Value,
            TotalValue = 0, 
            FormattedTotalValue = "R$ 0,00"
        });

        var recentProductDtos = recentProducts.Select(p =>
        {
            var dto = _mapper.Map<ProductDto>(p);
            dto.CategoryName = categoryLookup.GetValueOrDefault(p.CategoryId, "Unknown Category");
            return dto;
        });

        return new DashboardDto
        {
            Stats = stats,
            LowStockProducts = lowStockProductDtos,
            CategoryStats = categoryStats,
            RecentProducts = recentProductDtos
        };
    }
}