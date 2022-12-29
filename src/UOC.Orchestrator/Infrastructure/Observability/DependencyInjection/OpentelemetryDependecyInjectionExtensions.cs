using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Microsoft.Extensions.Logging;
using OpenTelemetry.Logs;
using System.Linq;
using Microsoft.Extensions.Configuration;
using UOC.SharedContracts;

namespace UOC.Orchestrator.Infrastructure.Observability
{
    public static class OpentelemetryDependecyInjectionExtensions
    {
        public static void AddObservability(this WebApplicationBuilder builder, IConfiguration configuration)
        {
            var observabilityConfig  = configuration.GetSection("Observability").Get<ObservabilityOptions>((a) => a.BindNonPublicProperties = true);
            var serviceName = typeof(OpentelemetryDependecyInjectionExtensions).Namespace.Split('.', 3).Take(2).Aggregate((a,b) => string.Format("{0}-{1}", a,b));
            
            builder.Services.AddOpenTelemetryTracing(builder =>
            {
                builder.SetResourceBuilder(
                    ResourceBuilder
                        .CreateDefault()
                        .AddService(serviceName))
                        .AddSource("MassTransit")
                        .AddAspNetCoreInstrumentation(options =>
                        {
                            options.RecordException = true;
                        })
                        .AddHttpClientInstrumentation(options =>
                        {
                            options.SetHttpFlavor = true;
                            options.RecordException = true;
                        })
                        .AddEntityFrameworkCoreInstrumentation(opts =>
                        {
                            opts.SetDbStatementForText = true;
                        })
                        .AddConsoleExporter()
                        .AddOtlpExporter(builder => { builder.Endpoint = observabilityConfig.EndpointAddress; });
                });
                
                builder.Logging.ClearProviders();
                builder.Logging.AddOpenTelemetry(option =>
                {
                    option.SetResourceBuilder(ResourceBuilder.CreateDefault().AddService(serviceName));
                    option.IncludeFormattedMessage = true;
                    option.IncludeScopes = true;
                    option.AddConsoleExporter();
                    option.AddOtlpExporter(builder => { builder.Endpoint = observabilityConfig.EndpointAddress; });
                });
        }
    }
}