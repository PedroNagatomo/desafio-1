using MediatR;
using Hypesoft.Application.DTOs;

namespace Hypesoft.Application.Commands.Products;

public class UpdateStockCommand : IRequest<ProductDto>
{
    public string ProductId { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public StockOperationType Operation { get; set; } = StockOperationType.Set;

    public UpdateStockCommand() { }

    public UpdateStockCommand(string productId, UpdateStockDto dto)
    {
        ProductId = productId;
        Quantity = dto.Quantity;
        Operation = dto.Operation;
    }
}