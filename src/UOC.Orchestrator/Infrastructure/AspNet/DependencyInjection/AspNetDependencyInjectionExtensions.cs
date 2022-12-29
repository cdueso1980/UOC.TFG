using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;

namespace UOC.Orchestrator.Infrastructure.AspNet
{
    public static class AspNetDependencyInjectionExtensions
    {
        public static IServiceCollection AddCustomHealthChecks(this IServiceCollection services)
        {
            services.AddHealthChecks()
                .AddCheck("liveness", () => HealthCheckResult.Healthy(), tags: new[] { "live" })
				.AddCheck("startup", () => HealthCheckResult.Healthy(), tags: new[] { "startup" })
				.AddCheck("ready", () => HealthCheckResult.Healthy(), tags: new[] { "ready" });
            return services;
        }

        public static IEndpointRouteBuilder UseCustomHealthChecks(this IEndpointRouteBuilder endpoints) 
        {
			endpoints.MapHealthChecks("/startup", new HealthCheckOptions() { Predicate = (check) => check.Tags.Contains("startup") });
            endpoints.MapHealthChecks("/live", new HealthCheckOptions() { Predicate = (check) => check.Tags.Contains("live") });
            endpoints.MapHealthChecks("/ready", new HealthCheckOptions() { Predicate = (check) => check.Tags.Contains("ready") }); 
			return endpoints;
		}
    }
}