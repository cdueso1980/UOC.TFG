using System;
using MassTransit;

namespace UOC.Messages
{
    public record OrderStatusChangedPaymentEvent : CorrelatedBy<Guid>
    {
        public Guid CorrelationId { get; init; }
        public Guid OrderId { get; init; }
        public Guid CustomerId { get; init; }
		public decimal TotalAmount { get; init; }
    }
}