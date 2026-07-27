using Microsoft.Extensions.DependencyInjection;

namespace ECommerce.UseCases;

public static  class DependencyInjection
{
    public static IServiceCollection AddUseCases(this IServiceCollection services)
    {
        return services;
    }
}
