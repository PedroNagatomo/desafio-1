using MediatR;
using Hypesoft.Application.DTOs;

namespace Hypesoft.Application.Queries.Products;

public class GetProductByIdQuery : IRequest<ProductDto?>
{
    public string Id { get; set; } = string.Empty;

    public GetProductByIdQuery() { }

    public GetProductByIdQuery(string id)
    {
        Id = id;
    }
}