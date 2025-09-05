using AutoMapper;
using MediatR;
using Hypesoft.Application.Queries.Products;
using Hypesoft.Application.DTOs;
using Hypesoft.Domain.Repositories;

namespace Hypesoft.Application.Handlers.Products;

public class GetLowStockProductsHandler : IRequestHandler<GetLowStockProductsQuery, IEnumerable<ProductDto>>
{
    private readonly IProductRepository _productRepository;
    private readonly ICategoryRepository _categoryRepository;
    private readonly IMapper _mapper;

    public GetLowStockProductsHandler(
        IProductRepository productRepository,
        ICategoryRepository categoryRepository,
        IMapper mapper)
    {
        _productRepository = productRepository;
        _categoryRepository = categoryRepository;
        _mapper = mapper;
    }

    public async Task<IEnumerable<ProductDto>> Handle(GetLowStockProductsQuery request, CancellationToken cancellationToken)
    {
        var products = await _productRepository.GetLowStockProductsAsync(request.Threshold, cancellationToken);

        // Buscar categorias para popular os nomes
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
        return products.Select(p =>
        {
            var dto = _mapper.Map<ProductDto>(p);
            dto.CategoryName = categories.GetValueOrDefault(p.CategoryId, "Unknown Category");
            return dto;
        });
    }
}