using MediatR;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using UOC.PaymentService.Application.Messages.Commands;
using UOC.PaymentService.Handlers.Pipelines;
using UOC.PaymentService.Application.Validators.Commands;

namespace UOC.PaymentService.Application
{
    public static class ApplicationDependencyInjectionExtensions
    {
        public static IServiceCollection AddValidators(this IServiceCollection services) 
        {
            services.AddScoped(typeof(IPipelineBehavior<,>), typeof(PipelineValidationCommand<,>));
            services.AddScoped<IValidator<PayOrderCommand>, PayOrderCommandValidator>();
            return services;
        }
    }

    public static class MediatorDependencyInjectionExtensions
    {
        public static IServiceCollection AddMediator(this IServiceCollection services) 
        {
            services.AddMediatR(typeof(MediatorDependencyInjectionExtensions).Assembly);
            return services;
        }
    }
}

