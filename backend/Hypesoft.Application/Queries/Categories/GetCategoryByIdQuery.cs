using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hypesoft.Application.DTOs;
using MediatR;

namespace Hypesoft.Application.Queries.Categories
{
    public class GetCategoryByIdQuery : IRequest<CategoryDto?>
    {
        public string Id { get; set; } = string.Empty;

        public GetCategoryByIdQuery() { }

        public GetCategoryByIdQuery(string id)
        {
            Id = id;
        }
    }
}