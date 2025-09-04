using FluentValidation;
using MediatR;
using System.Reflection;
using Hypesoft.Application.Mappings;
using Hypesoft.Application.Commands.Products;
using Hypesoft.Application.Validators;

namespace Hypesoft.API.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        // MediatR
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(typeof(CreateProductCommand).Assembly);
        });

        services.AddAutoMapper(typeof(AutoMapperProfile));

        services.AddValidatorsFromAssembly(typeof(CreateProductValidator).Assembly);

        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationPipelineBehavior<,>));

        return services;
    }

    public static IServiceCollection AddApiServices(this IServiceCollection services)
    {
        services.AddControllers()
            .ConfigureApiBehaviorOptions(options =>
            {
                options.InvalidModelStateResponseFactory = context =>
                {
                    var errors = context.ModelState
                        .Where(x => x.Value?.Errors.Count > 0)
                        .Select(x => new
                        {
                            PropertyName = x.Key,
                            ErrorMessage = x.Value?.Errors.FirstOrDefault()?.ErrorMessage
                        });

                    var response = new
                    {
                        Error = "Validation failed",
                        Details = errors
                    };

                    return new Microsoft.AspNetCore.Mvc.BadRequestObjectResult(response);
                };
            });
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new()
            {
                Title = "Hypesoft Product Management API",
                Version = "v1",
                Description = "API para gestão de produtos e categorias"
            });

            var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
            if (File.Exists(xmlPath))
            {
                c.IncludeXmlComments(xmlPath);
            }
        });

        services.AddCors(options =>
        {
            options.AddPolicy("AllowFrontend", builder =>
            {
                builder
                    .AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader();
            });
        });

        return services;
    }
}