using Serilog;
using Hypesoft.API.Extensions;
using Hypesoft.Infrastructure.Configuration;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using Hypesoft.Infrastructure.HealthChecks;

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

    // Authentication & Authorization
    builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        .AddJwtBearer(options =>
        {
            var keycloakSettings = builder.Configuration.GetSection("Keycloak");
            var authority = keycloakSettings["Authority"];
            var audience = keycloakSettings["Audience"];
            var requireHttps = keycloakSettings.GetValue<bool>("RequireHttpsMetadata", true);

            options.Authority = authority;
            options.Audience = audience;
            options.RequireHttpsMetadata = requireHttps;
            options.SaveToken = true;

            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = false, // Keycloak doesn't always include audience
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ClockSkew = TimeSpan.FromMinutes(5),
                NameClaimType = ClaimTypes.Name,
                RoleClaimType = ClaimTypes.Role
            };

            options.Events = new JwtBearerEvents
            {
                OnAuthenticationFailed = context =>
                {
                    Log.Warning("Authentication failed: {Error}", context.Exception.Message);
                    return Task.CompletedTask;
                },
                OnTokenValidated = context =>
                {
                    Log.Information("Token validated for user: {User}", 
                        context.Principal?.Identity?.Name ?? "Unknown");
                    return Task.CompletedTask;
                }
            };
        });

    builder.Services.AddAuthorization(options =>
    {
        options.AddPolicy("AdminOnly", policy => 
            policy.RequireClaim("realm_access.roles", "admin"));
        
        options.AddPolicy("ManagerOrAbove", policy => 
            policy.RequireAssertion(context =>
                context.User.HasClaim("realm_access.roles", "admin") ||
                context.User.HasClaim("realm_access.roles", "manager")));
        
        options.AddPolicy("UserOrAbove", policy => 
            policy.RequireAssertion(context =>
                context.User.HasClaim("realm_access.roles", "admin") ||
                context.User.HasClaim("realm_access.roles", "manager") ||
                context.User.HasClaim("realm_access.roles", "user")));
    });

    // Health checks
    builder.Services.AddHealthChecks()
        .AddCheck<MongoDbHealthCheck>("mongodb")
        .AddCheck<KeycloakHealthCheck>("keycloak");

    var app = builder.Build();

    // Configure the HTTP request pipeline
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI(c =>
        {
            c.SwaggerEndpoint("/swagger/v1/swagger.json", "Hypesoft API v1");
            c.RoutePrefix = string.Empty; // Swagger na raiz
            c.OAuthClientId("hypesoft-api");
            c.OAuthRealm("hypesoft");
            c.OAuthAppName("Hypesoft API");
        });
    }

    app.UseCustomMiddlewares();

    app.UseCors("AllowFrontend");

    app.UseAuthentication();
    app.UseAuthorization();

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