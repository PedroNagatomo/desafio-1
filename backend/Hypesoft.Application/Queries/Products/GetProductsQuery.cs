using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hypesoft.Application.DTOs;
using MediatR;

namespace Hypesoft.Application.Queries.Products
{
    public class GetProductsQuery : IRequest<PagedResultDto<ProductDto>>
    {
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string? SearchTerm { get; set; }
        public string? CategoryId { get; set; }
        public bool? IsActive { get; set; }
        public string? OrderBy { get; set; }
        public bool Ascending { get; set; } = true;

        public GetProductsQuery() { }

        public GetProductsQuery(int page, int pageSize, string? searchTerm = null, string? categoryId = null, bool? isActive = null, string? orderBy = null, bool ascending = true)
        {
            Page = Math.Max(1, page);
            PageSize = Math.Max(1, Math.Min(100, pageSize));
            SearchTerm = searchTerm;
            CategoryId = categoryId;
            IsActive = isActive;
            OrderBy = orderBy;
            Ascending = ascending;
        }
    }
}