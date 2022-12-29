using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using UOC.PaymentService.Domain;
using UOC.SharedContracts;

namespace UOC.PaymentService.Infrastructure.Persistence
{
    public static class PersistenceDependencyInjectionExtensions
    {
        public static IServiceCollection AddPersistence(this IServiceCollection services, IConfiguration configuration)
        {
            var postgresqlConfigOptions = configuration.GetSection("postgresql").Get<PostgresqlConfigOptions>();

            services.AddDbContext<PaymentDbContext>(options => options.UseNpgsql(postgresqlConfigOptions.Connection));
            services.AddSingleton(postgresqlConfigOptions);
            services.AddSingleton<IMigrationsRunner, MigrationsRunner>();
            services.AddScoped<IPaymentAggregateRepository, PaymentAggregateRepository>();
            services.AddScoped<ICustomerPaymentInfoAggregateRepository, CustomerPaymentInfoAggregateRepository>();

            return services;
        }
    }
}