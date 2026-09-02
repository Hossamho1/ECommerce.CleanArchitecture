using ECommerce.API.Filters;
using ECommerce.API.Middlewares;
using ECommerce.Infrastructure.Identity;
using ECommerce.UseCases.Common.Settings;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace ECommerce.API;

public static class DependencyInjection
{
      public static IServiceCollection AddApi(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddControllers(options=>
        options.Filters.Add<AuditActionFilter>()
        );
        services.AddHttpContextAccessor();
        // ICurrentUserService from UseCases is implemented in API layer
        services.AddScoped<ECommerce.UseCases.Common.Interfaces.ICurrentUserService, ECommerce.API.Services.CurrentUserService>();
        services.AddProblemDetails();
        services.AddExceptionHandler<GlobalExceptionMiddleware>();
        services.AddSwaggerGen();
        services.AddIdentity<ApplicationUser, ApplicationRole>(options =>
        {
            options.Password.RequiredLength = 8;

            options.User.RequireUniqueEmail = true;

            options.SignIn.RequireConfirmedEmail = true;
        })

                    .AddEntityFrameworkStores<IdentityStoreDbContext>()
            .AddDefaultTokenProviders();
         var jwtSettings = configuration.GetSection("Jwt").Get<JwtSettings>();

        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
     .AddJwtBearer(options =>
      {
    options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidIssuer = jwtSettings.Issuer,

        ValidateAudience = true,
        ValidAudience = jwtSettings.Audience,

        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Secret)),

        ValidateLifetime = true,

        ClockSkew = TimeSpan.FromMinutes(1)
    };
});

        services.AddAuthorization();
       
        return services;
    }
}
