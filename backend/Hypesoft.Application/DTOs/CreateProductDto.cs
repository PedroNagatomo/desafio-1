namespace Hypesoft.Application.DTOs;

public class CreateProductDto
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public string CategoryId { get; set; } = string.Empty;
    public int Stock { get; set; }
    public string? SKU { get; set; }
}