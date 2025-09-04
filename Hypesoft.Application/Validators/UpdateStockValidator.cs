using FluentValidation;
using Hypesoft.Application.Commands.Products;
using Hypesoft.Application.DTOs;

namespace Hypesoft.Application.Validators;

public class UpdateStockValidator : AbstractValidator<UpdateStockCommand>
{
    public UpdateStockValidator()
    {
        RuleFor(x => x.ProductId)
            .NotEmpty().WithMessage("Product ID is required");

        RuleFor(x => x.Quantity)
            .GreaterThan(0).WithMessage("Quantity must be greater than 0")
            .When(x => x.Operation == StockOperationType.Add || x.Operation == StockOperationType.Remove);

        RuleFor(x => x.Quantity)
            .GreaterThanOrEqualTo(0).WithMessage("Quantity cannot be negative")
            .When(x => x.Operation == StockOperationType.Set);

        RuleFor(x => x.Operation)
            .IsInEnum().WithMessage("Invalid stock operation type");
    }
}