using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;
using Hypesoft.Application.Commands.Products;
using Hypesoft.Domain.Repositories;

namespace Hypesoft.Application.Validators
{
    public class UpdateProductValidator : AbstractValidator<UpdateProductCommand>
    {
        private readonly ICategoryRepository _categoryRepository;

        public UpdateProductValidator(ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;

            RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Product ID is required");

            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Product name is required")
                .Length(1, 200).WithMessage("Product name must be between 1 and 200 characters");

            RuleFor(x => x.Description)
                .MaximumLength(1000).WithMessage("Description cannot exceed 1000 characters");

            RuleFor(x => x.Price)
                .GreaterThan(0).WithMessage("Price must be greater than 0")
                .LessThanOrEqualTo(999999.99m).WithMessage("Price cannot exceed 999,999.99");

            RuleFor(x => x.CategoryId)
                .NotEmpty().WithMessage("Category is required")
                .MustAsync(CategoryExists).WithMessage("Selected category does not exist");

            RuleFor(x => x.SKU)
                .MaximumLength(50).WithMessage("SKU cannot exceed 50 characters")
                .When(x => !string.IsNullOrWhiteSpace(x.SKU));
        }

        private async Task<bool> CategoryExists(string categoryId, CancellationToken cancellationToken)
        {
            var category = await _categoryRepository.GetByIdAsync(categoryId, cancellationToken);
            return category != null && category.IsActive;
        }
    }
}