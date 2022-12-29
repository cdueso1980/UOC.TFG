using System.IO;
using Microsoft.AspNetCore.Builder;
using UOC.Orchestrator.Infrastructure.AspNet;
using UOC.Orchestrator.Infrastructure.Persistence;
using UOC.Orchestrator.Infrastructure.Messaging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using UOC.Orchestrator.Infrastructure.Observability;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration
    .SetBasePath(builder.Environment.ContentRootPath)
    .AddJsonFile(Path.Combine(builder.Environment.ContentRootPath, "..", "appsettings-shared.json"))
    .AddEnvironmentVariables();
builder.Services.AddCustomHealthChecks();
builder.Services.AddPersistence(builder.Configuration);
builder.Services.AddMessaging(builder.Configuration);
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




