using MassTransit;
using UOC.SharedContracts;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using System;
using UOC.Messages;
using Microsoft.EntityFrameworkCore;
using MassTransit.EntityFrameworkCoreIntegration;
using UOC.Orchestrator.Application;
using UOC.Orchestrator.Domain;
using UOC.Orchestrator.Infrastructure.Persistence;

namespace UOC.Orchestrator.Infrastructure.Messaging
{
    public static class MessagingDependencyInjectionExtensions
    {
        public static IServiceCollection AddMessaging(this IServiceCollection services, IConfiguration configuration)
        {
            var messagingConfig  = configuration.GetSection("messaging").Get<MessagingOptions>((a) => a.BindNonPublicProperties = true);

            services.Configure<MassTransitHostOptions>(options => { options.WaitUntilStarted = true; });
            
            services.AddMassTransit(configurator =>
            {
                configurator.AddHealthChecks();

                var formatter = new EndpointNameFormatterUOC(messagingConfig.SagaConfiguration.SagaPattern);
                configurator.SetEndpointNameFormatter(formatter);

                configurator.AddSagaStateMachine<OrderPaymentStateMachineSaga, OrderPaymentState>(sagaConfig => sagaConfig.UseInMemoryOutbox());

                configurator.SetEntityFrameworkSagaRepositoryProvider(efConfig => 
                {
                    efConfig.ExistingDbContext<OrchestratorSagaPersistenceDbContext>();
                    efConfig.LockStatementProvider = new PostgresLockStatementProvider();
                    efConfig.ConcurrencyMode =  ConcurrencyMode.Optimistic;
                });

                configurator.UsingRabbitMq((context, config) => 
                {
                    config.UseNewtonsoftJsonDeserializer();

                    config.Host(messagingConfig.RabbitConfiguration.BuildUri(), cf => 
                    {
                        cf.PublisherConfirmation = true;
                    });

                    config.Message<OrderStatusChangedPaymentEvent>(x => { x.SetEntityNameFormatter(new MessageNameFormatterUOC<OrderStatusChangedPaymentEvent>()); });
                    config.Message<PayOrderCompletedEvent>(x => { x.SetEntityNameFormatter(new MessageNameFormatterUOC<PayOrderCompletedEvent>()); });
                    config.Message<PayOrderRejectedEvent>(x => { x.SetEntityNameFormatter(new MessageNameFormatterUOC<PayOrderRejectedEvent>()); });
                    config.Message<OrderStatusChangedPaymentRejectedEvent>(x => { x.SetEntityNameFormatter(new MessageNameFormatterUOC<OrderStatusChangedPaymentRejectedEvent>()); });
                    config.Message<OrderStatusChangedCompletedEvent>(x => { x.SetEntityNameFormatter(new MessageNameFormatterUOC<OrderStatusChangedCompletedEvent>()); });
                    config.Message<OrderToPayCommand>(x => { x.SetEntityNameFormatter(new MessageNameFormatterUOC<OrderToPayCommand>());  });
                    config.Message<OrderToRejectCommand>(x => { x.SetEntityNameFormatter(new MessageNameFormatterUOC<OrderToRejectCommand>());  });
                    config.Message<OrderToCompleteCommand>(x => { x.SetEntityNameFormatter(new MessageNameFormatterUOC<OrderToCompleteCommand>());  });

                    config.ReceiveEndpoint(formatter.Saga<OrderPaymentState>(), e => 
                    {   
                        const int ConcurrencyLimit = 20; 
                        e.PrefetchCount = ConcurrencyLimit;
                        e.UseMessageRetry(r => r.Interval(5, 1000));
                        e.UseInMemoryOutbox();
                        e.ConfigureSaga<OrderPaymentState>(context, s => { });
			        });
                });
            });

            services.AddMessagingConventions(messagingConfig);

            return services;
        }

        private static IServiceCollection AddMessagingConventions(this IServiceCollection services, MessagingOptions messagingConfig) 
        {
			EndpointConvention.Map<OrderToPayCommand>(new Uri($"queue:{messagingConfig.QueueConfiguration.OrderPayment}"));
			EndpointConvention.Map<OrderToRejectCommand>(new Uri($"queue:{messagingConfig.QueueConfiguration.OrderCancel}"));
			EndpointConvention.Map<OrderToCompleteCommand>(new Uri($"queue:{messagingConfig.QueueConfiguration.OrderConfirm}"));

			return services;
		}
    }
}