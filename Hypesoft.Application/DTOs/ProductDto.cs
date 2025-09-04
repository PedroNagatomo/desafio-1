namespace Hypesoft.Application.DTOs
{
    public class ProductDto
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public string Currency { get; set; } = "BRL";
        public string CategoryId { get; set; } = string.Empty;
        public string? CategoryName { get; set; }
        public int Stock { get; set; }
        public bool IsActive { get; set; }
        public string? SKU { get; set; }
        public bool IsLowStock { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public string FormattedPrice { get; set; } = string.Empty;
        public string StockStatus { get; set; } = string.Empty;
    }
}