using MediatR;
using Microsoft.AspNetCore.Mvc;
using Hypesoft.Application.Commands.Categories;
using Hypesoft.Application.Queries.Categories;
using Hypesoft.Application.DTOs;

namespace Hypesoft.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class CategoriesController : ControllerBase
{
    private readonly IMediator _mediator;

    public CategoriesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Obtém uma lista paginada de categorias
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<PagedResultDto<CategoryDto>>> GetCategories(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? search = null,
        [FromQuery] bool? isActive = null,
        CancellationToken cancellationToken = default)
    {
        var query = new GetCategoriesQuery(page, pageSize, search, isActive);
        var result = await _mediator.Send(query, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Obtém uma categoria específica por ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<CategoryDto>> GetCategory(string id, CancellationToken cancellationToken = default)
    {
        var query = new GetCategoryByIdQuery(id);
        var result = await _mediator.Send(query, cancellationToken);
        
        if (result == null)
            return NotFound(new { Message = $"Category with id '{id}' not found" });
        
        return Ok(result);
    }

    /// <summary>
    /// Cria uma nova categoria
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<CategoryDto>> CreateCategory(
        CreateCategoryDto dto,
        CancellationToken cancellationToken = default)
    {
        var command = new CreateCategoryCommand(dto);
        var result = await _mediator.Send(command, cancellationToken);
        return CreatedAtAction(nameof(GetCategory), new { id = result.Id }, result);
    }

    /// <summary>
    /// Atualiza uma categoria existente
    /// </summary>
    [HttpPut("{id}")]
    public async Task<ActionResult<CategoryDto>> UpdateCategory(
        string id,
        UpdateCategoryDto dto,
        CancellationToken cancellationToken = default)
    {
        var command = new UpdateCategoryCommand(id, dto);
        var result = await _mediator.Send(command, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Remove uma categoria
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteCategory(string id, CancellationToken cancellationToken = default)
    {
        var command = new DeleteCategoryCommand(id);
        var result = await _mediator.Send(command, cancellationToken);
        
        if (!result)
            return NotFound(new { Message = $"Category with id '{id}' not found" });
        
        return NoContent();
    }
}