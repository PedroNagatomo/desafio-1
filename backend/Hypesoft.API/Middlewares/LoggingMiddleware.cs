using System.Diagnostics;

namespace Hypesoft.API.Middlewares;

public class LoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<LoggingMiddleware> _logger;

    public LoggingMiddleware(RequestDelegate next, ILogger<LoggingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var correlationId = Guid.NewGuid().ToString();
        context.Items["CorrelationId"] = correlationId;
        
        var stopwatch = Stopwatch.StartNew();
        
        _logger.LogInformation("Request started: {Method} {Path} - CorrelationId: {CorrelationId}",
            context.Request.Method, context.Request.Path, correlationId);

        try
        {
            await _next(context);
        }
        finally
        {
            stopwatch.Stop();
            _logger.LogInformation("Request completed: {Method} {Path} - Status: {StatusCode} - Duration: {ElapsedMs}ms - CorrelationId: {CorrelationId}",
                context.Request.Method, context.Request.Path, context.Response.StatusCode, stopwatch.ElapsedMilliseconds, correlationId);
        }
    }
}