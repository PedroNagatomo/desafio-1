using Serilog;
using Hypesoft.API.Extensions;
using Hypesoft.Infrastructure.Configuration;

var builder = WebApplication.CreateBuilder(args);

// Serilog Configuration
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .CreateLogger();

builder.Host.UseSerilog();

try
{
    Log.Information("Starting Hypesoft API");

    // Add services to the container
    builder.Services.AddApplication();
    builder.Services.AddInfrastructure(builder.Configuration);
    builder.Services.AddApiServices();

    // Health checks
    builder.Services.AddHealthChecks();

    var app = builder.Build();

    // Configure the HTTP request pipeline
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI(c =>
        {
            c.SwaggerEndpoint("/swagger/v1/swagger.json", "Hypesoft API v1");
            c.RoutePrefix = string.Empty; // Swagger na raiz
        });
    }

    app.UseCustomMiddlewares();

    app.UseCors("AllowFrontend");

    app.UseRouting();

    app.MapControllers();
    app.MapHealthChecks("/health");

    // Initialize database
    await app.InitializeDatabaseAsync();

    Log.Information("Hypesoft API started successfully");
    
    await app.RunAsync();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}