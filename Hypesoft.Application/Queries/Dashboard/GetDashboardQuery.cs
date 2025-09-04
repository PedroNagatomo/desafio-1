using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hypesoft.Application.DTOs;
using MediatR;

namespace Hypesoft.Application.Queries.Dashboard
{
    public class GetDashboardQuery : IRequest<DashboardDto>
    {
        public int LowStockThreshold { get; set; } = 10;
        public int RecentProductsCount { get; set; } = 5;

        public GetDashboardQuery() { }

        public GetDashboardQuery(int lowStockThreshold, int recentProductsCount)
        {
            LowStockThreshold = Math.Max(0, lowStockThreshold);
            RecentProductsCount = Math.Max(1, Math.Min(20, recentProductsCount));
        }
    }
}