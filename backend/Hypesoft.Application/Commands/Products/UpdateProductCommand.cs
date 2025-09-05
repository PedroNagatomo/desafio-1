using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hypesoft.Application.DTOs;
using MediatR;

namespace Hypesoft.Application.Commands.Products
{
    public class UpdateProductCommand : IRequest<ProductDto>
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public string CategoryId { get; set; } = string.Empty;
        public string? SKU { get; set; }

        public UpdateProductCommand() { }

        public UpdateProductCommand(string id, UpdateProductDto dto)
        {
            Id = id;
            Name = dto.Name;
            Description = dto.Description;
            Price = dto.Price;
            CategoryId = dto.CategoryId;
            SKU = dto.SKU;
        }
    }
}