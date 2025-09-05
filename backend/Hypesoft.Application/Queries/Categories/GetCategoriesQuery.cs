using MediatR;
using Hypesoft.Application.DTOs;

namespace Hypesoft.Application.Queries.Categories;

public class GetCategoriesQuery : IRequest<PagedResultDto<CategoryDto>>
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public string? SearchTerm { get; set; }
    public bool? IsActive { get; set; }

    public GetCategoriesQuery() { }

    public GetCategoriesQuery(int page, int pageSize, string? searchTerm = null, bool? isActive = null)
    {
        Page = Math.Max(1, page);
        PageSize = Math.Max(1, Math.Min(100, pageSize));
        SearchTerm = searchTerm;
        IsActive = isActive;
    }
}