using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;
using Hypesoft.Application.Commands.Categories;
using Hypesoft.Domain.Repositories;

namespace Hypesoft.Application.Validators
{
    public class UpdateCategoryValidator : AbstractValidator<UpdateCategoryCommand>
    {
        private readonly ICategoryRepository _categoryRepository;

        public UpdateCategoryValidator(ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;

            RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Category ID is required");

            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Category name is required")
                .Length(1, 100).WithMessage("Category name must be between 1 and 100 characters")
                .MustAsync(BeUniqueCategoryName).WithMessage("Category name already exists");

            RuleFor(x => x.Description)
                .MaximumLength(500).WithMessage("Description cannot exceed 500 characters");
        }

        private async Task<bool> BeUniqueCategoryName(UpdateCategoryCommand command, string name, CancellationToken cancellationToken)
        {
            return !await _categoryRepository.ExistsByNameAsync(name, command.Id, cancellationToken);
        }
    }
}