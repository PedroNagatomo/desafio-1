using MediatR;
using Hypesoft.Application.Commands.Categories;
using Hypesoft.Domain.Repositories;
using Hypesoft.Domain.Exceptions;

namespace Hypesoft.Application.Handlers.Categories;

public class DeleteCategoryHandler : IRequestHandler<DeleteCategoryCommand, bool>
{
    private readonly ICategoryRepository _categoryRepository;
    private readonly IProductRepository _productRepository;

    public DeleteCategoryHandler(ICategoryRepository categoryRepository, IProductRepository productRepository)
    {
        _categoryRepository = categoryRepository;
        _productRepository = productRepository;
    }

    public async Task<bool> Handle(DeleteCategoryCommand request, CancellationToken cancellationToken)
    {
        var category = await _categoryRepository.GetByIdAsync(request.Id, cancellationToken);
        if (category == null)
        {
            return false;
        }

        // Verificar se a categoria tem produtos associados
        if (await _productRepository.ExistsAsync(p => p.CategoryId == request.Id, cancellationToken))
        {
            throw new CategoryInUseException(category.Name);
        }

        await _categoryRepository.DeleteAsync(category, cancellationToken);
        return true;
    }
}