using System;
using MassTransit;
using UOC.SharedContracts;
using UOC.Messages;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;

namespace UOC.OrderService.Infrastructure.Messaging
{
    public static class MessagingDependencyInjectionExtensions
    {
        public static IServiceCollection AddMessaging(this IServiceCollection services, IConfiguration configuration)
        {
            var messagingConfig  = configuration.GetSection("messaging").Get<MessagingOptions>((a) => a.BindNonPublicProperties = true);
            
            services.AddMassTransit(configurator =>
            {
                configurator.SetEndpointNameFormatter(new EndpointNameFormatterUOC(messagingConfig.SagaConfiguration.SagaPattern));

                configurator.AddConsumer<OrderToRejectCommandConsumer>();
                configurator.AddConsumer<OrderToCompleteCommandConsumer>();

                configurator.UsingRabbitMq((context, config) => 
                {
                    config.UseNewtonsoftJsonDeserializer();
                    
                    config.UseMessageRetry(r => r.Incremental(5, TimeSpan.FromMilliseconds(500), TimeSpan.FromSeconds(1)));

                    config.Host(messagingConfig.RabbitConfiguration.BuildUri(), cf => 
                    {
                        cf.PublisherConfirmation = true;
                    });

                    config.Message<OrderStatusChangedPaymentEvent>(x => { x.SetEntityNameFormatter(new MessageNameFormatterUOC<OrderStatusChangedPaymentEvent>()); });
                    config.Message<OrderStatusChangedCompletedEvent>(x => { x.SetEntityNameFormatter(new MessageNameFormatterUOC<OrderStatusChangedCompletedEvent>()); });
                    config.Message<OrderStatusChangedPaymentRejectedEvent>(x => { x.SetEntityNameFormatter(new MessageNameFormatterUOC<OrderStatusChangedPaymentRejectedEvent>()); });

                    config.Message<OrderToRejectCommand>(x => { x.SetEntityNameFormatter(new MessageNameFormatterUOC<OrderToRejectCommand>());  });
                    config.Message<OrderToCompleteCommand>(x => { x.SetEntityNameFormatter(new MessageNameFormatterUOC<OrderToCompleteCommand>());  });

                    config.ReceiveEndpoint(messagingConfig.QueueConfiguration.OrderConfirm, e => 
                    {
                        e.ConfigureConsumer<OrderToCompleteCommandConsumer>(context);
			        });

                    config.ReceiveEndpoint(messagingConfig.QueueConfiguration.OrderCancel, e => 
                    {
                        e.ConfigureConsumer<OrderToRejectCommandConsumer>(context);
			        });
                });
            });

            return services;
        }
    }
}