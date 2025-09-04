using MediatR;
using Hypesoft.Application.DTOs;

namespace Hypesoft.Application.Commands.Products;

public class CreateProductCommand : IRequest<ProductDto>
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public string CategoryId { get; set; } = string.Empty;
    public int Stock { get; set; }
    public string? SKU { get; set; }

    public CreateProductCommand() { }

    public CreateProductCommand(CreateProductDto dto)
    {
        Name = dto.Name;
        Description = dto.Description;
        Price = dto.Price;
        CategoryId = dto.CategoryId;
        Stock = dto.Stock;
        SKU = dto.SKU;
    }
}