using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Configuration;

namespace Hypesoft.Infrastructure.HealthChecks;

public class KeycloakHealthCheck : IHealthCheck
{
    private readonly IConfiguration _configuration;
    private readonly HttpClient _httpClient;

    public KeycloakHealthCheck(IConfiguration configuration, HttpClient httpClient)
    {
        _configuration = configuration;
        _httpClient = httpClient;
    }

    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context, 
        CancellationToken cancellationToken = default)
    {
        try
        {
            var keycloakUrl = _configuration["Keycloak:Authority"];
            if (string.IsNullOrEmpty(keycloakUrl))
            {
                return HealthCheckResult.Unhealthy("Keycloak URL not configured");
            }

            var response = await _httpClient.GetAsync($"{keycloakUrl}/.well-known/openid_configuration", cancellationToken);
            
            if (response.IsSuccessStatusCode)
            {
                return HealthCheckResult.Healthy("Keycloak is accessible");
            }
            
            return HealthCheckResult.Degraded($"Keycloak returned status: {response.StatusCode}");
        }
        catch (Exception ex)
        {
            return HealthCheckResult.Unhealthy("Keycloak is not accessible", ex);
        }
    }
}