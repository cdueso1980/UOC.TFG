using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using UOC.SharedContracts;

namespace UOC.Orchestrator.Infrastructure.Persistence
{
    public static class PersistenceDependencyInjectionExtensions
    {
        public static IServiceCollection AddPersistence(this IServiceCollection services, IConfiguration configuration)
        {
            var postgresqlConfigOptions = configuration.GetSection("postgresql").Get<PostgresqlConfigOptions>();

            services.AddDbContext<OrchestratorSagaPersistenceDbContext>(builder => builder.UseNpgsql(postgresqlConfigOptions.Connection, m => { }));
            services.AddSingleton(postgresqlConfigOptions);
            services.AddSingleton<IMigrationsRunner, MigrationsRunner>();

            return services;
        }
    }
}