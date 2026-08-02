using ECommerce.API.Middlewares;

namespace ECommerce.API;

public static class DependencyInjection
{
      public static IServiceCollection AddApi(this IServiceCollection services)
    {
        services.AddControllers();
        services.AddProblemDetails();
        services.AddExceptionHandler<GlobalExceptionMiddleware>();
        services.AddSwaggerGen();
        return services;
    }
}
