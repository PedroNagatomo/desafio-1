using AutoMapper;
using MediatR;
using Hypesoft.Application.Queries.Categories;
using Hypesoft.Application.DTOs;
using Hypesoft.Domain.Repositories;

namespace Hypesoft.Application.Handlers.Categories;

public class GetCategoryByIdHandler : IRequestHandler<GetCategoryByIdQuery, CategoryDto?>
{
    private readonly ICategoryRepository _categoryRepository;
    private readonly IProductRepository _productRepository;
    private readonly IMapper _mapper;

    public GetCategoryByIdHandler(
        ICategoryRepository categoryRepository,
        IProductRepository productRepository,
        IMapper mapper)
    {
        _categoryRepository = categoryRepository;
        _productRepository = productRepository;
        _mapper = mapper;
    }

    public async Task<CategoryDto?> Handle(GetCategoryByIdQuery request, CancellationToken cancellationToken)
    {
        var category = await _categoryRepository.GetByIdAsync(request.Id, cancellationToken);
        if (category == null)
            return null;

        var categoryDto = _mapper.Map<CategoryDto>(category);

        // Contar produtos da categoria
        categoryDto.ProductCount = (int)await _productRepository.CountAsync(p => p.CategoryId == category.Id, cancellationToken);

        return categoryDto;
    }
}