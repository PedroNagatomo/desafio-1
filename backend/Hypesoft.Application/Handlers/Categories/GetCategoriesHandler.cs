using AutoMapper;
using MediatR;
using Hypesoft.Application.Queries.Categories;
using Hypesoft.Application.DTOs;
using Hypesoft.Domain.Repositories;

namespace Hypesoft.Application.Handlers.Categories;

public class GetCategoriesHandler : IRequestHandler<GetCategoriesQuery, PagedResultDto<CategoryDto>>
{
    private readonly ICategoryRepository _categoryRepository;
    private readonly IProductRepository _productRepository;
    private readonly IMapper _mapper;

    public GetCategoriesHandler(
        ICategoryRepository categoryRepository,
        IProductRepository productRepository,
        IMapper mapper)
    {
        _categoryRepository = categoryRepository;
        _productRepository = productRepository;
        _mapper = mapper;
    }

    public async Task<PagedResultDto<CategoryDto>> Handle(GetCategoriesQuery request, CancellationToken cancellationToken)
    {
        var (categories, totalCount) = await _categoryRepository.GetPagedAsync(
            request.Page,
            request.PageSize,
            request.SearchTerm,
            request.IsActive,
            cancellationToken);

        var productCounts = await _productRepository.GetProductCountByCategoryAsync(cancellationToken);

        var categoryDtos = categories.Select(c =>
        {
            var dto = _mapper.Map<CategoryDto>(c);
            dto.ProductCount = productCounts.GetValueOrDefault(c.Id, 0);
            return dto;
        });

        return new PagedResultDto<CategoryDto>(categoryDtos, request.Page, request.PageSize, totalCount);
    }
}