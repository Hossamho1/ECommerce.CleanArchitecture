using ECommerce.Application.Behaviors;
using ECommerce.Application.Brands.Queries;
using ECommerce.Application.Products.Queries;
using ECommerce.Application.Types;
using ECommerce.Application.Types.Queries;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace ECommerce.UseCases;

public static class DependencyInjection
{
    public static IServiceCollection AddUseCases(this IServiceCollection services)
    {
        // Register MediatR handlers from this assembly (use Brand query as anchor)
        services.AddMediatR(cfg => {
            cfg.RegisterServicesFromAssembly(typeof(GetAllBrandQuery).Assembly);
        });
        // Register FluentValidation validators from this assembly
        services.AddValidatorsFromAssembly(typeof(GetAllBrandQuery).Assembly);

        // Register MediatR pipeline behaviors for validation
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

        // Keep existing non-MediatR use-case registrations if needed
        services.AddScoped<GetAllProductsQuery>();
        services.AddScoped<GetByIdProductsQuery>();

        services.AddScoped<GetAllTypesQuery>();
        services.AddScoped<GetByIdTypeQuery>();

        return services;
    }
}
