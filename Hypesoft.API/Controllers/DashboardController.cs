using MediatR;
using Microsoft.AspNetCore.Mvc;
using Hypesoft.Application.Queries.Dashboard;
using Hypesoft.Application.DTOs;

namespace Hypesoft.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class DashboardController : ControllerBase
{
    private readonly IMediator _mediator;

    public DashboardController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Obtém dados do dashboard
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<DashboardDto>> GetDashboard(
        [FromQuery] int lowStockThreshold = 10,
        [FromQuery] int recentProductsCount = 5,
        CancellationToken cancellationToken = default)
    {
        var query = new GetDashboardQuery(lowStockThreshold, recentProductsCount);
        var result = await _mediator.Send(query, cancellationToken);
        return Ok(result);
    }
}