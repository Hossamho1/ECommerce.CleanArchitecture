using ECommerce.API;
using ECommerce.Infrastructure;
using ECommerce.Infrastructure.Data.DbContexts;
using ECommerce.Infrastructure.Persistence.Seeding;
using ECommerce.UseCases;
using Microsoft.EntityFrameworkCore;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApi();

builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddUseCases();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    await using var scope = app.Services.CreateAsyncScope();

    var dbSeed = scope.ServiceProvider.GetRequiredService<DatabaseSeeder>();
    var dbContext = scope.ServiceProvider.GetRequiredService<StoreDbContext>();

    await dbContext.Database.MigrateAsync();

    await dbSeed.SeedAll();
}

app.Run();

