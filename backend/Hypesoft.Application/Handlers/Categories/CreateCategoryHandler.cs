using AutoMapper;
using MediatR;
using Hypesoft.Application.Commands.Categories;
using Hypesoft.Application.DTOs;
using Hypesoft.Domain.Entities;
using Hypesoft.Domain.Repositories;
using Hypesoft.Domain.Exceptions;

namespace Hypesoft.Application.Handlers.Categories;

public class CreateCategoryHandler : IRequestHandler<CreateCategoryCommand, CategoryDto>
{
    private readonly ICategoryRepository _categoryRepository;
    private readonly IMapper _mapper;

    public CreateCategoryHandler(ICategoryRepository categoryRepository, IMapper mapper)
    {
        _categoryRepository = categoryRepository;
        _mapper = mapper;
    }

    public async Task<CategoryDto> Handle(CreateCategoryCommand request, CancellationToken cancellationToken)
    {
        if (await _categoryRepository.ExistsByNameAsync(request.Name, cancellationToken: cancellationToken))
        {
            throw new DuplicateCategoryException(request.Name);
        }

        var category = new Category(request.Name, request.Description);

        var createdCategory = await _categoryRepository.AddAsync(category, cancellationToken);

        return _mapper.Map<CategoryDto>(createdCategory);
    }
}