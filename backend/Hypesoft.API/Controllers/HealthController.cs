using Microsoft.AspNetCore.Mvc;
using Hypesoft.Infrastructure.Data;
using MongoDB.Driver;

namespace Hypesoft.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class HealthController : ControllerBase
{
    private readonly MongoDbContext _context;

    public HealthController(MongoDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Health check da aplicação
    /// </summary>
    [HttpGet]
    public async Task<ActionResult> GetHealth()
    {
        try
        {
            // Teste de conexão com MongoDB
            await _context.Products.CountDocumentsAsync(FilterDefinition<Domain.Entities.Product>.Empty);
            
            return Ok(new
            {
                Status = "Healthy",
                Timestamp = DateTime.UtcNow,
                Version = "1.0.0",
                Database = "Connected"
            });
        }
        catch (Exception ex)
        {
            return StatusCode(503, new
            {
                Status = "Unhealthy",
                Timestamp = DateTime.UtcNow,
                Error = ex.Message,
                Database = "Disconnected"
            });
        }
    }
}