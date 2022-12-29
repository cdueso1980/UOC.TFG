using MassTransit;
using UOC.SharedContracts;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using System;
using UOC.Messages;

namespace UOC.PaymentService.Infrastructure.Messaging
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

                configurator.SetEndpointNameFormatter(new EndpointNameFormatterUOC(messagingConfig.SagaConfiguration.SagaPattern));

                configurator.AddConsumer<OrderToPayCommandConsumer>();

                configurator.UsingRabbitMq((context, config) => 
                {
                    config.UseNewtonsoftJsonDeserializer();

                    config.UseMessageRetry(r => r.Incremental(5, TimeSpan.FromMilliseconds(500), TimeSpan.FromSeconds(1)));

                    config.Host(messagingConfig.RabbitConfiguration.BuildUri(), cf => 
                    {
                        cf.PublisherConfirmation = true;
                    });
                    
                    config.Message<OrderToPayCommand>(x => { x.SetEntityNameFormatter(new MessageNameFormatterUOC<OrderToPayCommand>());  });
                    config.Message<PayOrderCompletedEvent>(x => { x.SetEntityNameFormatter(new MessageNameFormatterUOC<PayOrderCompletedEvent>()); });
                    config.Message<PayOrderRejectedEvent>(x => { x.SetEntityNameFormatter(new MessageNameFormatterUOC<PayOrderRejectedEvent>()); });

                    config.ReceiveEndpoint(messagingConfig.QueueConfiguration.OrderPayment, e => 
                    {
                        e.ConfigureConsumer<OrderToPayCommandConsumer>(context);
			        });
                });
            });

            return services;
        }
    }
}