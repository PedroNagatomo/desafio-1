using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hypesoft.Application.Commands.Products;
using Hypesoft.Application.DTOs;
using Hypesoft.Application.Queries.Products;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Hypesoft.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
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
    /// Cria um novo produto
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<ProductDto>> CreateProduct(
        CreateProductDto dto,
        CancellationToken cancellationToken = default)
    {
        var command = new CreateProductCommand(dto);
        var result = await _mediator.Send(command, cancellationToken);
        return CreatedAtAction(nameof(GetProduct), new { id = result.Id }, result);
    }

    /// <summary>
    /// Atualiza um produto existente
    /// </summary>
    [HttpPut("{id}")]
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
    /// Remove um produto
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteProduct(string id, CancellationToken cancellationToken = default)
    {
        var command = new DeleteProductCommand(id);
        var result = await _mediator.Send(command, cancellationToken);

        if (!result)
            return NotFound(new { Message = $"Product with id '{id}' not found" });

        return NoContent();
    }

    /// <summary>
    /// Atualiza o estoque de um produto
    /// </summary>
    [HttpPatch("{id}/stock")]
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