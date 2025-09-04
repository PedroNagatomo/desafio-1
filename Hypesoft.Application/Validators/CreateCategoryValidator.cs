using FluentValidation;
using Hypesoft.Application.Commands.Categories;
using Hypesoft.Domain.Repositories;

namespace Hypesoft.Application.Validators;

public class CreateCategoryValidator : AbstractValidator<CreateCategoryCommand>
{
    private readonly ICategoryRepository _categoryRepository;

    public CreateCategoryValidator(ICategoryRepository categoryRepository)
    {
        _categoryRepository = categoryRepository;

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Category name is required")
            .Length(1, 100).WithMessage("Category name must be between 1 and 100 characters")
            .MustAsync(BeUniqueCategoryName).WithMessage("Category name already exists");

        RuleFor(x => x.Description)
            .MaximumLength(500).WithMessage("Description cannot exceed 500 characters");
    }

    private async Task<bool> BeUniqueCategoryName(string name, CancellationToken cancellationToken)
    {
        return !await _categoryRepository.ExistsByNameAsync(name, cancellationToken: cancellationToken);
    }
}