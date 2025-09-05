namespace Hypesoft.Application.DTOs;

public class CategoryStatsDto
{
    public string CategoryId { get; set; } = string.Empty;
    public string CategoryName { get; set; } = string.Empty;
    public int ProductCount { get; set; }
    public decimal TotalValue { get; set; }
    public string FormattedTotalValue { get; set; } = string.Empty;
}
