using FluentValidation;
using Hypesoft.Application.Queries.Categories;

namespace Hypesoft.Application.Validators;

public class GetCategoriesQueryValidator : AbstractValidator<GetCategoriesQuery>
{
    public GetCategoriesQueryValidator()
    {
        RuleFor(x => x.Page)
            .GreaterThan(0).WithMessage("Page must be greater than 0");

        RuleFor(x => x.PageSize)
            .GreaterThan(0).WithMessage("PageSize must be greater than 0")
            .LessThanOrEqualTo(100).WithMessage("PageSize cannot exceed 100");

        RuleFor(x => x.SearchTerm)
            .MaximumLength(200).WithMessage("Search term cannot exceed 200 characters")
            .When(x => !string.IsNullOrWhiteSpace(x.SearchTerm));
    }
}