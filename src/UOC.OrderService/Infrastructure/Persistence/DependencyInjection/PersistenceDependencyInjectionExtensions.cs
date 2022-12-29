using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using UOC.OrderService.Domain;
using UOC.SharedContracts;

namespace UOC.OrderService.Infrastructure.Persistence
{
    public static class PersistenceDependencyInjectionExtensions
    {
        public static IServiceCollection AddPersistence(this IServiceCollection services, IConfiguration configuration)
        {
            var postgresqlConfigOptions = configuration.GetSection("postgresql").Get<PostgresqlConfigOptions>();

            services.AddDbContext<OrderDbContext>(options => options.UseNpgsql(postgresqlConfigOptions.Connection));
            services.AddSingleton(postgresqlConfigOptions);
            services.AddSingleton<IMigrationsRunner, MigrationsRunner>();
            services.AddScoped<IOrderAggregateRepository, OrderAggregateRepository>();

            return services;
        }
    }
}