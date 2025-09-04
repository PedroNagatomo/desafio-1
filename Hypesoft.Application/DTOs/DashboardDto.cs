using Hypesoft.Application.DTOs;

namespace Hypesoft.Application.DTOs;

public class DashboardDto
{
    public DashboardStatsDto Stats { get; set; } = new();
    public IEnumerable<ProductDto> LowStockProducts { get; set; } = new List<ProductDto>();
    public IEnumerable<CategoryStatsDto> CategoryStats { get; set; } = new List<CategoryStatsDto>();
    public IEnumerable<ProductDto> RecentProducts { get; set; } = new List<ProductDto>();
}