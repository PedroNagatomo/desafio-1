using AutoMapper;
using MediatR;
using Hypesoft.Application.Queries.Products;
using Hypesoft.Application.DTOs;
using Hypesoft.Domain.Repositories;
using System.Linq.Expressions;
using Hypesoft.Domain.Entities;

namespace Hypesoft.Application.Handlers.Products;

public class GetProductsHandler : IRequestHandler<GetProductsQuery, PagedResultDto<ProductDto>>
{
    private readonly IProductRepository _productRepository;
    private readonly ICategoryRepository _categoryRepository;
    private readonly IMapper _mapper;

    public GetProductsHandler(
        IProductRepository productRepository,
        ICategoryRepository categoryRepository,
        IMapper mapper)
    {
        _productRepository = productRepository;
        _categoryRepository = categoryRepository;
        _mapper = mapper;
    }

    public async Task<PagedResultDto<ProductDto>> Handle(GetProductsQuery request, CancellationToken cancellationToken)
    {
        
        Expression<Func<Product, bool>>? filter = null;
        
        if (!string.IsNullOrWhiteSpace(request.SearchTerm) || 
            !string.IsNullOrWhiteSpace(request.CategoryId) || 
            request.IsActive.HasValue)
        {
            filter = p => (string.IsNullOrWhiteSpace(request.SearchTerm) || 
                          p.Name.ToLower().Contains(request.SearchTerm.ToLower())) &&
                         (string.IsNullOrWhiteSpace(request.CategoryId) || 
                          p.CategoryId == request.CategoryId) &&
                         (!request.IsActive.HasValue || p.IsActive == request.IsActive.Value);
        }

        
        Expression<Func<Product, object>>? orderBy = request.OrderBy?.ToLower() switch
        {
            "name" => p => p.Name,
            "price" => p => p.Price.Amount,
            "stock" => p => p.Stock.Value,
            "createdat" => p => p.createdAt,
            _ => p => p.createdAt
        };

        
        var (products, totalCount) = await _productRepository.GetPagedAsync(
            request.Page,
            request.PageSize,
            filter,
            orderBy,
            request.Ascending,
            cancellationToken);

        var categoryIds = products.Select(p => p.CategoryId).Distinct().ToList();
        var categories = new Dictionary<string, string>();
        
        if (categoryIds.Any())
        {
            var categoryList = await _categoryRepository.FindAsync(
                c => categoryIds.Contains(c.Id), 
                cancellationToken);
            categories = categoryList.ToDictionary(c => c.Id, c => c.Name);
        }

        // Mapear para DTOs
        var productDtos = products.Select(p =>
        {
            var dto = _mapper.Map<ProductDto>(p);
            dto.CategoryName = categories.GetValueOrDefault(p.CategoryId, "Unknown Category");
            return dto;
        });

        return new PagedResultDto<ProductDto>(productDtos, request.Page, request.PageSize, totalCount);
    }
}