using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Hypesoft.Application.Commands.Products;
using Hypesoft.Application.Queries.Products;
using Hypesoft.Application.DTOs;

namespace Hypesoft.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
[Authorize(Policy = "UserOrAbove")]
public class ProductsController : ControllerBase
{
    private readonly IMediator _mediator;

    public ProductsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Obtém uma lista paginada de produtos
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<PagedResultDto<ProductDto>>> GetProducts(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? search = null,
        [FromQuery] string? categoryId = null,
        [FromQuery] bool? isActive = null,
        [FromQuery] string? orderBy = null,
        [FromQuery] bool ascending = true,
        CancellationToken cancellationToken = default)
    {
        var query = new GetProductsQuery(page, pageSize, search, categoryId, isActive, orderBy, ascending);
        var result = await _mediator.Send(query, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Obtém um produto específico por ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<ProductDto>> GetProduct(string id, CancellationToken cancellationToken = default)
    {
        var query = new GetProductByIdQuery(id);
        var result = await _mediator.Send(query, cancellationToken);
        
        if (result == null)
            return NotFound(new { Message = $"Product with id '{id}' not found" });
        
        return Ok(result);
    }

    /// <summary>
    /// Cria um novo produto (apenas Managers e Admins)
    /// </summary>
    [HttpPost]
    [Authorize(Policy = "ManagerOrAbove")]
    public async Task<ActionResult<ProductDto>> CreateProduct(
        CreateProductDto dto, 
        CancellationToken cancellationToken = default)
    {
        var command = new CreateProductCommand(dto);
        var result = await _mediator.Send(command, cancellationToken);
        return CreatedAtAction(nameof(GetProduct), new { id = result.Id }, result);
    }

    /// <summary>
    /// Atualiza um produto existente (apenas Managers e Admins)
    /// </summary>
    [HttpPut("{id}")]
    [Authorize(Policy = "ManagerOrAbove")]
    public async Task<ActionResult<ProductDto>> UpdateProduct(
        string id,
        UpdateProductDto dto,
        CancellationToken cancellationToken = default)
    {
        var command = new UpdateProductCommand(id, dto);
        var result = await _mediator.Send(command, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Remove um produto (apenas Admins)
    /// </summary>
    [HttpDelete("{id}")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<ActionResult> DeleteProduct(string id, CancellationToken cancellationToken = default)
    {
        var command = new DeleteProductCommand(id);
        var result = await _mediator.Send(command, cancellationToken);
        
        if (!result)
            return NotFound(new { Message = $"Product with id '{id}' not found" });
        
        return NoContent();
    }

    /// <summary>
    /// Atualiza o estoque de um produto (apenas Managers e Admins)
    /// </summary>
    [HttpPatch("{id}/stock")]
    [Authorize(Policy = "ManagerOrAbove")]
    public async Task<ActionResult<ProductDto>> UpdateStock(
        string id,
        UpdateStockDto dto,
        CancellationToken cancellationToken = default)
    {
        var command = new UpdateStockCommand(id, dto);
        var result = await _mediator.Send(command, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Obtém produtos com estoque baixo
    /// </summary>
    [HttpGet("low-stock")]
    public async Task<ActionResult<IEnumerable<ProductDto>>> GetLowStockProducts(
        [FromQuery] int threshold = 10,
        CancellationToken cancellationToken = default)
    {
        var query = new GetLowStockProductsQuery(threshold);
        var result = await _mediator.Send(query, cancellationToken);
        return Ok(result);
    }
}