using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Hypesoft.Application.Commands.Categories;
using Hypesoft.Application.Commands.Products;
using Hypesoft.Application.DTOs;
using Hypesoft.Domain.Entities;
using Hypesoft.Domain.ValueObjects;

namespace Hypesoft.Application.Mappings
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap();
        }

        private void CreateMap()
        {
            CreateMap<Product, ProductDto>()
                .ForMember(dest => dest.Price, opt => opt.MapFrom(src => src.Price.Amount))
                .ForMember(dest => dest.Currency, opt => opt.MapFrom(src => src.Price.Currency))
                .ForMember(dest => dest.Stock, opt => opt.MapFrom(src => src.Stock.Value))
                .ForMember(dest => dest.IsLowStock, opt => opt.MapFrom(src => src.IsLowStock(10)))
                .ForMember(dest => dest.FormattedPrice, opt => opt.MapFrom(src => src.Price.ToString()))
                .ForMember(dest => dest.StockStatus, opt => opt.MapFrom(src => GetStockStatus(src.Stock.Value)))
                .ForMember(dest => dest.CategoryName, opt => opt.Ignore());

            CreateMap<CreateProductCommand, Product>()
                .ConstructUsing(src => new Product(
                    src.Name,
                    src.Description,
                    new Money(src.Price, "BRL"),
                    src.CategoryId,
                    new StockQuantity(src.Stock),
                    src.SKU
                ));

            CreateMap<CreateProductDto, CreateProductCommand>();
            CreateMap<UpdateProductDto, UpdateProductCommand>();

            CreateMap<Category, CategoryDto>()
                .ForMember(dest => dest.ProductCount, opt => opt.Ignore());

            CreateMap<CreateCategoryCommand, Category>()
                .ConstructUsing(src => new Category(src.Name, src.Description));

            CreateMap<CreateCategoryDto, CreateCategoryCommand>();
            CreateMap<UpdateCategoryDto, UpdateCategoryCommand>();

            CreateMap<DashboardStatsDto, DashboardStatsDto>()
            .ForMember(dest => dest.FormattedTotalStockValue,
                opt => opt.MapFrom(src => $"R$ {src.TotalStockValue:N2}"));

            CreateMap<CategoryStatsDto, CategoryStatsDto>()
                .ForMember(dest => dest.FormattedTotalValue,
                    opt => opt.MapFrom(src => $"R$ {src.TotalValue:N2}"));

            // Value Object Mappings
            CreateMap<Money, decimal>()
                .ConvertUsing(src => src.Amount);

            CreateMap<decimal, Money>()
                .ConvertUsing(src => new Money(src, "BRL"));

            CreateMap<StockQuantity, int>()
                .ConvertUsing(src => src.Value);

            CreateMap<int, StockQuantity>()
                .ConvertUsing(src => new StockQuantity(src));
        }

        private static string GetStockStatus(int stockValue)
        {
            return stockValue switch
            {
                0 => "Out of stock",
                < 10 => "Low stock",
                < 50 => "In stock",
                _ => "Well stocked"
            };
        }
    }
}