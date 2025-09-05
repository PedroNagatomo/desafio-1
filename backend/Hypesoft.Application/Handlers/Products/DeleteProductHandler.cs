using MediatR;
using Hypesoft.Application.Commands.Products;
using Hypesoft.Domain.Repositories;

namespace Hypesoft.Application.Handlers.Products;

public class DeleteProductHandler : IRequestHandler<DeleteProductCommand, bool>
{
    private readonly IProductRepository _productRepository;

    public DeleteProductHandler(IProductRepository productRepository)
    {
        _productRepository = productRepository;
    }

    public async Task<bool> Handle(DeleteProductCommand request, CancellationToken cancellationToken)
    {
        var product = await _productRepository.GetByIdAsync(request.Id, cancellationToken);
        if (product == null)
        {
            return false;
        }

        await _productRepository.DeleteAsync(product, cancellationToken);
        return true;
    }
}
