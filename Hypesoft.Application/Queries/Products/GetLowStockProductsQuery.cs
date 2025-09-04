using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hypesoft.Application.DTOs;
using MediatR;

namespace Hypesoft.Application.Queries.Products
{
    public class GetLowStockProductsQuery : IRequest<IEnumerable<ProductDto>>
    {
        public int Threshold { get; set; } = 10;
        public GetLowStockProductsQuery() { }

        public GetLowStockProductsQuery(int threshold)
        {
            Threshold = Math.Max(0, threshold);
        }
    }
}