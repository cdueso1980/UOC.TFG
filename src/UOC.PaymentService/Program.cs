using System.IO;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using UOC.PaymentService.Infrastructure.AspNet;
using UOC.PaymentService.Infrastructure.Messaging;
using Microsoft.Extensions.DependencyInjection;
using UOC.PaymentService.Infrastructure.Persistence;
using UOC.PaymentService.Application;
using UOC.PaymentService.Infrastructure.Domain;
using UOC.PaymentService.Infrastructure.Observability;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration
    .SetBasePath(builder.Environment.ContentRootPath)
    .AddJsonFile(Path.Combine(builder.Environment.ContentRootPath, "..", "appsettings-shared.json"))
    .AddEnvironmentVariables();
    
builder.Services.AddCustomHealthChecks();
builder.Services.AddMessaging(builder.Configuration);
builder.Services.AddPersistence(builder.Configuration);
builder.Services.AddMediator();
builder.Services.AddValidators();
builder.Services.AddDomainServices();
builder.AddObservability(builder.Configuration);

var app = builder.Build();

app.UseRouting();
app.UseEndpoints(endpoints =>
{
    endpoints.UseCustomHealthChecks();
});

var migrationRunner = app.Services.GetRequiredService<IMigrationsRunner>();
migrationRunner.Migrate();
await app.RunAsync();