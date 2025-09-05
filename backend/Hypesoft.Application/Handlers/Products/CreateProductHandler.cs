using AutoMapper;
using MediatR;
using Hypesoft.Application.Commands.Products;
using Hypesoft.Application.DTOs;
using Hypesoft.Domain.Entities;
using Hypesoft.Domain.Repositories;
using Hypesoft.Domain.ValueObjects;
using Hypesoft.Domain.Exceptions;

namespace Hypesoft.Application.Handlers.Products;

public class CreateProductHandler : IRequestHandler<CreateProductCommand, ProductDto>
{
    private readonly IProductRepository _productRepository;
    private readonly ICategoryRepository _categoryRepository;
    private readonly IMapper _mapper;

    public CreateProductHandler(
        IProductRepository productRepository,
        ICategoryRepository categoryRepository,
        IMapper mapper)
    {
        _productRepository = productRepository;
        _categoryRepository = categoryRepository;
        _mapper = mapper;
    }

    public async Task<ProductDto> Handle(CreateProductCommand request, CancellationToken cancellationToken)
    {
        // Verificar se a categoria existe
        var category = await _categoryRepository.GetByIdAsync(request.CategoryId, cancellationToken);
        if (category == null || !category.IsActive)
        {
            throw new CategoryNotFoundException(request.CategoryId);
        }

        // Verificar se o nome do produto já existe
        if (await _productRepository.ExistsByNameAsync(request.Name, cancellationToken: cancellationToken))
        {
            throw new DuplicateProductException(request.Name);
        }

        // Verificar se o SKU já existe (se fornecido)
        if (!string.IsNullOrWhiteSpace(request.SKU) && 
            await _productRepository.ExistsBySKUAsync(request.SKU, cancellationToken: cancellationToken))
        {
            throw new DuplicateProductException($"SKU '{request.SKU}' already exists");
        }

        // Criar o produto
        var product = new Product(
            request.Name,
            request.Description,
            new Money(request.Price),
            request.CategoryId,
            new StockQuantity(request.Stock),
            request.SKU);

        // Salvar no repositório
        var createdProduct = await _productRepository.AddAsync(product, cancellationToken);

        // Mapear para DTO
        var productDto = _mapper.Map<ProductDto>(createdProduct);
        productDto.CategoryName = category.Name;

        return productDto;
    }
}