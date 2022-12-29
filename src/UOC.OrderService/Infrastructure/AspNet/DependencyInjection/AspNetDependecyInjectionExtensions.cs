using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Diagnostics;
using UOC.OrderService.Validators;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Http;

namespace UOC.OrderService.Infrastructure.AspNet
{
    public static class AspNetDependecyInjectionExtensions
    {
        public static IServiceCollection AddCustomHealthChecks(this IServiceCollection services)
        {
            services.AddHealthChecks()
                .AddCheck("liveness", () => HealthCheckResult.Healthy(), tags: new[] { "live" })
				.AddCheck("startup", () => HealthCheckResult.Healthy(), tags: new[] { "startup" })
				.AddCheck("ready", () => HealthCheckResult.Healthy(), tags: new[] { "ready" });

            return services;
        }

        public static WebApplication UseCustomHHealthChecks(this WebApplication webApplication) 
        {
			webApplication.MapHealthChecks("/startup", new HealthCheckOptions() { Predicate = (check) => check.Tags.Contains("startup") });
            webApplication.MapHealthChecks("/live", new HealthCheckOptions() { Predicate = (check) => check.Tags.Contains("live") });
            webApplication.MapHealthChecks("/ready", new HealthCheckOptions() { Predicate = (check) => check.Tags.Contains("ready") }); 
			return webApplication;
		}

        public static WebApplication UseCustomExceptionHandler(this WebApplication webApplication) 
        {
			webApplication.UseExceptionHandler(appBuilder => 
            {
                appBuilder.Run(async httpContext => 
                {
                    var exceptionHandlerPathFeature = httpContext.Features.Get<IExceptionHandlerPathFeature>();

                    httpContext.Response.StatusCode = exceptionHandlerPathFeature.Error is CommandValidationException ? 400 : 500;
                    httpContext.Response.ContentType = "application/json";

                    appBuilder.ApplicationServices.GetRequiredService<ILogger<Program>>().LogError(exceptionHandlerPathFeature.Error, exceptionHandlerPathFeature.Error.Message);

                    var response = new ApiResponseBase()
                    {
                        Errors = new [] { exceptionHandlerPathFeature.Error.Message },
                        WaitForResponse = false,
                        Processed = false
                    }; 

                    var result =  JsonConvert.SerializeObject(response);
                    await httpContext.Response.WriteAsync(result);
                });
            });

			return webApplication;
		}
    }
}