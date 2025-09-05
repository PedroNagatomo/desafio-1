using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hypesoft.Domain.Entities;
using Hypesoft.Domain.Repositories;
using Hypesoft.Domain.ValueObjects;

namespace Hypesoft.Infrastructure.Data
{
    public class DatabaseSeeder
    {
        private readonly ICategoryRepository _categoryRepository;
    private readonly IProductRepository _productRepository;

    public DatabaseSeeder(ICategoryRepository categoryRepository, IProductRepository productRepository)
    {
        _categoryRepository = categoryRepository;
        _productRepository = productRepository;
    }

    public async Task SeedAsync()
    {
        var existingCategories = await _categoryRepository.CountAsync();
        if (existingCategories > 0)
            return;

        var categories = new[]
        {
            new Category("Eletrônicos", "Dispositivos eletrônicos e acessórios"),
            new Category("Roupas", "Vestuário masculino e feminino"),
            new Category("Casa & Jardim", "Itens para casa e jardim"),
            new Category("Esportes", "Artigos esportivos e fitness"),
            new Category("Livros", "Livros e material educativo"),
            new Category("Beleza", "Produtos de beleza e cuidados pessoais")
        };

        await _categoryRepository.AddRangeAsync(categories);

        var products = new[]
        {
            new Product("iPhone 14", "Smartphone Apple iPhone 14 128GB", new Money(4999.99m), 
                categories[0].Id, new StockQuantity(25), "IPH14-128"),
            
            new Product("Samsung Galaxy S23", "Smartphone Samsung Galaxy S23 256GB", new Money(3899.99m), 
                categories[0].Id, new StockQuantity(15), "SGS23-256"),
            
            new Product("Notebook Dell", "Notebook Dell Inspiron 15 3000", new Money(2499.99m), 
                categories[0].Id, new StockQuantity(8), "DELL-I15"),
            
            new Product("Camiseta Básica", "Camiseta 100% algodão", new Money(39.99m), 
                categories[1].Id, new StockQuantity(100), "CAM-BAS-001"),
            
            new Product("Calça Jeans", "Calça jeans masculina", new Money(129.99m), 
                categories[1].Id, new StockQuantity(45), "JEANS-M-001"),
            
            new Product("Sofá 3 Lugares", "Sofá confortável para sala", new Money(1299.99m), 
                categories[2].Id, new StockQuantity(5), "SOFA-3L-001"),
            
            new Product("Bicicleta Mountain Bike", "Bicicleta para trilhas", new Money(899.99m), 
                categories[3].Id, new StockQuantity(12), "BIKE-MTB-001"),
            
            new Product("Clean Code", "Livro sobre código limpo", new Money(89.99m), 
                categories[4].Id, new StockQuantity(30), "BOOK-CC-001"),
            
            new Product("Shampoo Premium", "Shampoo para todos os tipos de cabelo", new Money(25.99m), 
                categories[5].Id, new StockQuantity(75), "SHAM-PREM-001")
        };

        await _productRepository.AddRangeAsync(products);
    }
    }
}