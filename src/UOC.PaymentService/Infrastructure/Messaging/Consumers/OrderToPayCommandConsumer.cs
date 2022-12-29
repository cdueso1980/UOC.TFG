using System;
using System.Threading.Tasks;
using UOC.Messages;
using MassTransit;
using MediatR;
using UOC.PaymentService.Application.Messages.Commands;

namespace UOC.PaymentService.Infrastructure.Messaging
{
    public class OrderToPayCommandConsumer : IConsumer<OrderToPayCommand>
    {
        private readonly IMediator mediator;

        public OrderToPayCommandConsumer(IMediator mediator)
        {
            this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public async Task Consume(ConsumeContext<OrderToPayCommand> context)
        {   
            await this.mediator.Send(new PayOrderCommand() 
            { 
                OrderId = context.Message.OrderId, 
                CorrelationId = context.Message.CorrelationId,
                CustomerId = context.Message.CustomerId,
                TotalAmount = context.Message.TotalAmount
            });
        }
    }
}