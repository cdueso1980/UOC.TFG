using System;
using MediatR;
using MassTransit;
using UOC.Messages;
using System.Threading.Tasks;
using UOC.OrderService.Application.Messages.Commands;

namespace UOC.OrderService.Infrastructure.Messaging
{
    public class OrderToCompleteCommandConsumer : IConsumer<OrderToCompleteCommand>
    {
        private readonly IMediator mediator;

        public OrderToCompleteCommandConsumer(IMediator mediator)
        {
            this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public async Task Consume(ConsumeContext<OrderToCompleteCommand> context)
        {   
            await this.mediator.Send(new OrderChangeStatusToCompleteCommand() { OrderId = context.Message.OrderId, CorrelationId = context.Message.CorrelationId });
        }
    }
}