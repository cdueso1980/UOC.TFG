using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using UOC.PaymentService.Domain;
using UOC.PaymentService.Infrastructure.Domain.Services;

namespace UOC.PaymentService.Infrastructure.Domain
{
    public static class DomainDependencyInjectionExtensions
    {
        public static IServiceCollection AddDomainServices(this IServiceCollection services)
        {
            services.AddSingleton<IThirdPartyPaymentService, ThirdPartyPaymentService>();
            return services;
        }
    }
} 