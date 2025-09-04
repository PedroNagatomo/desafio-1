using Hypesoft.API.Middlewares;
using Hypesoft.Infrastructure.Configuration;

namespace Hypesoft.API.Extensions;

public static class ApplicationBuilderExtensions
{
    public static IApplicationBuilder UseCustomMiddlewares(this IApplicationBuilder app)
    {
        app.UseMiddleware<LoggingMiddleware>();
        app.UseMiddleware<ExceptionMiddleware>();
        
        return app;
    }

    public static async Task<IApplicationBuilder> InitializeDatabaseAsync(this IApplicationBuilder app)
    {
        await app.ApplicationServices.InitializeDatabaseAsync();
        return app;
    }
}