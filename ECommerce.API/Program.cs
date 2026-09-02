using ECommerce.API;
using ECommerce.Infrastructure;
using ECommerce.Infrastructure.Data.DbContexts;
using ECommerce.Infrastructure.Persistence.Seeding;
using ECommerce.UseCases;
using ECommerce.Infrastructure.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApi(builder.Configuration);
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddUseCases();

var app = builder.Build();

app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "ECommerce API V1");
        c.RoutePrefix = "swagger";
    });

    await using var scope = app.Services.CreateAsyncScope();

    var dbSeed = scope.ServiceProvider.GetRequiredService<DatabaseSeeder>();

    var dbContext = scope.ServiceProvider.GetRequiredService<StoreDbContext>();
    await dbContext.Database.MigrateAsync();

    var identityContext = scope.ServiceProvider.GetRequiredService<IdentityStoreDbContext>();
    await identityContext.Database.MigrateAsync();

    await dbSeed.SeedAll();
}
app.UseAuthentication();
app.UseAuthorization();


app.MapControllers();

app.Run();