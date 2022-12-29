using System;
using MassTransit;

namespace UOC.Messages
{
    public record OrderStatusChangedPaymentRejectedEvent : CorrelatedBy<Guid>
    {
        public Guid CorrelationId { get; init; }
        public Guid OrderId { get; init; }
    }
}