using System.IO;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using UOC.OrderService.Infrastructure.Messaging;
using UOC.OrderService.Infrastructure.Persistence;
using Microsoft.Extensions.Configuration;
using UOC.OrderService.Infrastructure.AspNet;
using UOC.OrderService.Application;
using UOC.OrderService.Infrastructure.Observability;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration
    .SetBasePath(builder.Environment.ContentRootPath)
    .AddJsonFile(Path.Combine(builder.Environment.ContentRootPath, "..", "appsettings-shared.json"))
    .AddEnvironmentVariables();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddMessaging(builder.Configuration);
builder.Services.AddCustomHealthChecks();
builder.Services.AddPersistence(builder.Configuration);
builder.Services.AddMediator();
builder.Services.AddValidators();
builder.AddObservability(builder.Configuration);

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();
app.UseHttpsRedirection();
app.MapControllers();
app.UseCustomHHealthChecks();
app.UseCustomExceptionHandler();

var migrationRunner = app.Services.GetRequiredService<IMigrationsRunner>();
migrationRunner.Migrate();
await app.RunAsync();