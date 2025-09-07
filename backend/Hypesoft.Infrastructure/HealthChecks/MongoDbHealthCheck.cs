using Hypesoft.Infrastructure.Data;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using MongoDB.Driver;

namespace Hypesoft.Infrastructure.HealthChecks;

public class MongoDbHealthCheck : IHealthCheck
{
    private readonly MongoDbContext _context;

    public MongoDbHealthCheck(MongoDbContext context)
    {
        _context = context;
    }

    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context, 
        CancellationToken cancellationToken = default)
    {
        try
        {
            await _context.Products.CountDocumentsAsync(
                FilterDefinition<Domain.Entities.Product>.Empty, 
                cancellationToken: cancellationToken);
            
            return HealthCheckResult.Healthy("MongoDB is accessible");
        }
        catch (Exception ex)
        {
            return HealthCheckResult.Unhealthy("MongoDB is not accessible", ex);
        }
    }
}