using MediatR;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using UOC.OrderService.Application.Messages.Commands;
using UOC.OrderService.Handlers.Pipelines;
using UOC.OrderService.Application.Validators.Commands;

namespace UOC.OrderService.Application
{
    public static class ApplicationDependencyInjectionExtensions
    {
        public static IServiceCollection AddValidators(this IServiceCollection services) 
        {
            services.AddScoped(typeof(IPipelineBehavior<,>), typeof(PipelineValidationCommand<,>));
            services.AddScoped<IValidator<OrderChangeStatusToPaymentCommand>, OrderChangeStatusToPaymentCommandValidator>();
            services.AddScoped<IValidator<OrderChangeStatusToPaymentRejectedCommand>, OrderChangeStatusToPaymentRejectedCommandValidator>();
            services.AddScoped<IValidator<OrderChangeStatusToCompleteCommand>, OrderChangeStatusToCompleteCommandValidator>();
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

