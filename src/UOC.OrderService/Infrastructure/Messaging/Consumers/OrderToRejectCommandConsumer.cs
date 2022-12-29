using System;
using MediatR;
using MassTransit;
using UOC.Messages;
using System.Threading.Tasks;
using UOC.OrderService.Application.Messages.Commands;

namespace UOC.OrderService.Infrastructure.Messaging
{
    public class OrderToRejectCommandConsumer : IConsumer<OrderToRejectCommand>
    {
        private readonly IMediator mediator;

        public OrderToRejectCommandConsumer(IMediator mediator)
        {
            this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public async Task Consume(ConsumeContext<OrderToRejectCommand> context)
        {   
            await this.mediator.Send(new OrderChangeStatusToPaymentRejectedCommand() { OrderId = context.Message.OrderId, CorrelationId = context.Message.CorrelationId });
        }
    }
}