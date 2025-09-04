using MediatR;
using Hypesoft.Application.DTOs;

namespace Hypesoft.Application.Commands.Categories;

public class CreateCategoryCommand : IRequest<CategoryDto>
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }

    public CreateCategoryCommand() { }

    public CreateCategoryCommand(CreateCategoryDto dto)
    {
        Name = dto.Name;
        Description = dto.Description;
    }
}