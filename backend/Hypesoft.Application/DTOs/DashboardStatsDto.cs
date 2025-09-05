namespace Hypesoft.Application.DTOs;

public class DashboardStatsDto
{
    public int TotalProducts { get; set; }
    public int TotalCategories { get; set; }
    public int LowStockProducts { get; set; }
    public decimal TotalStockValue { get; set; }
    public string FormattedTotalStockValue { get; set; } = string.Empty;
    public int ActiveProducts { get; set; }
    public int InactiveProducts { get; set; }
}