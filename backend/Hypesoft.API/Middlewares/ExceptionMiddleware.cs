using System.Net;
using System.Text.Json;
using Hypesoft.Domain.Exceptions;
using FluentValidation;

namespace Hypesoft.API.Middlewares;

public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionMiddleware> _logger;

    public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred: {Message}", ex.Message);
            await HandleExceptionAsync(context, ex);
        }
    }

    private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";

        var response = exception switch
        {
            ValidationException validationEx => new ErrorResponse
            {
                Error = "Validation failed",
                Details = validationEx.Errors.Select(e => new { e.PropertyName, e.ErrorMessage }),
                StatusCode = (int)HttpStatusCode.BadRequest
            },
            DomainException domainEx => new ErrorResponse
            {
                Error = domainEx.Message,
                StatusCode = (int)HttpStatusCode.BadRequest
            },
            ArgumentException => new ErrorResponse
            {
                Error = exception.Message,
                StatusCode = (int)HttpStatusCode.BadRequest
            },
            InvalidOperationException => new ErrorResponse
            {
                Error = exception.Message,
                StatusCode = (int)HttpStatusCode.BadRequest
            },
            _ => new ErrorResponse
            {
                Error = "An internal server error occurred",
                StatusCode = (int)HttpStatusCode.InternalServerError
            }
        };

        context.Response.StatusCode = response.StatusCode;

        var jsonResponse = JsonSerializer.Serialize(response, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        await context.Response.WriteAsync(jsonResponse);
    }
}