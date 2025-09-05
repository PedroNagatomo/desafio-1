using AutoMapper;
using MediatR;
using Hypesoft.Application.Commands.Products;
using Hypesoft.Application.DTOs;
using Hypesoft.Domain.Repositories;
using Hypesoft.Domain.Exceptions;

namespace Hypesoft.Application.Handlers.Products;

public class UpdateStockHandler : IRequestHandler<UpdateStockCommand, ProductDto>
{
    private readonly IProductRepository _productRepository;
    private readonly ICategoryRepository _categoryRepository;
    private readonly IMapper _mapper;

    public UpdateStockHandler(
        IProductRepository productRepository,
        ICategoryRepository categoryRepository,
        IMapper mapper)
    {
        _productRepository = productRepository;
        _categoryRepository = categoryRepository;
        _mapper = mapper;
    }

    public async Task<ProductDto> Handle(UpdateStockCommand request, CancellationToken cancellationToken)
    {
        var product = await _productRepository.GetByIdAsync(request.ProductId, cancellationToken);
        if (product == null)
        {
            throw new ProductNotFoundException(request.ProductId);
        }

        switch (request.Operation)
        {
            case StockOperationType.Set:
                product.UpdateStock(request.Quantity);
                break;
            case StockOperationType.Add:
                product.AddStock(request.Quantity);
                break;
            case StockOperationType.Remove:
                product.RemoveStock(request.Quantity);
                break;
            default:
                throw new ArgumentException("Invalid stock operation type");
        }

        await _productRepository.UpdateAsync(product, cancellationToken);

        var category = await _categoryRepository.GetByIdAsync(product.CategoryId, cancellationToken);

        var productDto = _mapper.Map<ProductDto>(product);
        productDto.CategoryName = category?.Name ?? "Unknown Category";

        return productDto;
    }
}