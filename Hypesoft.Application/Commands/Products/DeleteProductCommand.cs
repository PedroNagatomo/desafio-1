using MediatR;

namespace Hypesoft.Application.Commands.Products;

public class DeleteProductCommand : IRequest<bool>
{
    public string Id { get; set; } = string.Empty;

    public DeleteProductCommand() { }

    public DeleteProductCommand(string id)
    {
        Id = id;
    }
}