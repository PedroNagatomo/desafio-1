using AutoMapper;
using MediatR;
using Hypesoft.Application.Commands.Products;
using Hypesoft.Application.DTOs;
using Hypesoft.Domain.Repositories;
using Hypesoft.Domain.ValueObjects;
using Hypesoft.Domain.Exceptions;

namespace Hypesoft.Application.Handlers.Products;

public class UpdateProductHandler : IRequestHandler<UpdateProductCommand, ProductDto>
{
    private readonly IProductRepository _productRepository;
    private readonly ICategoryRepository _categoryRepository;
    private readonly IMapper _mapper;

    public UpdateProductHandler(
        IProductRepository productRepository,
        ICategoryRepository categoryRepository,
        IMapper mapper)
    {
        _productRepository = productRepository;
        _categoryRepository = categoryRepository;
        _mapper = mapper;
    }

    public async Task<ProductDto> Handle(UpdateProductCommand request, CancellationToken cancellationToken)
    {
        var product = await _productRepository.GetByIdAsync(request.Id, cancellationToken);
        if (product == null)
        {
            throw new ProductNotFoundException(request.Id);
        }

        var category = await _categoryRepository.GetByIdAsync(request.CategoryId, cancellationToken);
        if (category == null || !category.IsActive)
        {
            throw new CategoryNotFoundException(request.CategoryId);
        }

        if (await _productRepository.ExistsByNameAsync(request.Name, request.Id, cancellationToken))
        {
            throw new DuplicateProductException(request.Name);
        }

        if (!string.IsNullOrWhiteSpace(request.SKU) && 
            request.SKU != product.SKU &&
            await _productRepository.ExistsBySKUAsync(request.SKU, request.Id, cancellationToken))
        {
            throw new DuplicateProductException($"SKU '{request.SKU}' already exists");
        }

        product.UpdateInfo(
            request.Name,
            request.Description,
            new Money(request.Price),
            request.CategoryId,
            request.SKU);

        // Salvar no repositório
        await _productRepository.UpdateAsync(product, cancellationToken);

        // Mapear para DTO
        var productDto = _mapper.Map<ProductDto>(product);
        productDto.CategoryName = category.Name;

        return productDto;
    }
}
