using AutoMapper;
using MediatR;
using Hypesoft.Application.Commands.Categories;
using Hypesoft.Application.DTOs;
using Hypesoft.Domain.Repositories;
using Hypesoft.Domain.Exceptions;

namespace Hypesoft.Application.Handlers.Categories;

public class UpdateCategoryHandler : IRequestHandler<UpdateCategoryCommand, CategoryDto>
{
    private readonly ICategoryRepository _categoryRepository;
    private readonly IMapper _mapper;

    public UpdateCategoryHandler(ICategoryRepository categoryRepository, IMapper mapper)
    {
        _categoryRepository = categoryRepository;
        _mapper = mapper;
    }

    public async Task<CategoryDto> Handle(UpdateCategoryCommand request, CancellationToken cancellationToken)
    {
        // Verificar se a categoria existe
        var category = await _categoryRepository.GetByIdAsync(request.Id, cancellationToken);
        if (category == null)
        {
            throw new CategoryNotFoundException(request.Id);
        }

        // Verificar se o nome da categoria já existe (excluindo a categoria atual)
        if (await _categoryRepository.ExistsByNameAsync(request.Name, request.Id, cancellationToken))
        {
            throw new DuplicateCategoryException(request.Name);
        }

        // Atualizar a categoria
        category.UpdateInfo(request.Name, request.Description);

        // Salvar no repositório
        await _categoryRepository.UpdateAsync(category, cancellationToken);

        // Mapear para DTO
        return _mapper.Map<CategoryDto>(category);
    }
}