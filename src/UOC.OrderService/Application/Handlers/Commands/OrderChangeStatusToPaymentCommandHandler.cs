using System;
using MassTransit;
using MediatR;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using UOC.OrderService.Application.Messages.Commands;
using UOC.OrderService.Domain;
using UOC.Messages;

namespace UOC.OrderService.Application.Handlers.Commands
{
    public sealed class OrderChangeStatusToPaymentCommandHandler : IRequestHandler<OrderChangeStatusToPaymentCommand, bool>
    {
        private readonly ILogger logger;
        private readonly IBus bus;
        private readonly IOrderAggregateRepository orderAggregateRepository;

        public OrderChangeStatusToPaymentCommandHandler(
            ILogger<OrderChangeStatusToPaymentCommandHandler> logger, 
            IBus bus, 
            IOrderAggregateRepository orderAggregateRepository)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.bus = bus ?? throw new ArgumentNullException(nameof(bus));
            this.orderAggregateRepository = orderAggregateRepository ?? throw new ArgumentNullException(nameof(orderAggregateRepository));
        }

        public async Task<bool> Handle(OrderChangeStatusToPaymentCommand request, CancellationToken cancellationToken)
        {
            var order = await this.orderAggregateRepository.Get(request.OrderId);
            if(order == null)
            {
                this.logger.LogWarning($"OrderId {request.OrderId} not found");
                return false;
            }

            order.ChangeStatusToPayment();

            await this.orderAggregateRepository.Save();
            //TODO: Use Outbox pattern for this operation!!!
            await this.bus.Publish(new OrderStatusChangedPaymentEvent 
            { 
                CorrelationId = request.CorrelationId, 
                OrderId = request.OrderId,
                CustomerId = order.CustomerId,
                TotalAmount = order.GetTotalAmount()
            }, cancellationToken);
            this.logger.LogInformation($"OrderId {request.OrderId} Change Status To Payment");
            return true;
        }
    }
}
