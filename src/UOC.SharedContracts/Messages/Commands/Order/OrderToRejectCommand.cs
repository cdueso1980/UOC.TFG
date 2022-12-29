using System;
using MassTransit;

namespace UOC.Messages
{
    public record OrderToRejectCommand : CorrelatedBy<Guid>
    {
        public Guid CorrelationId { get; init; }
        public Guid OrderId { get; init; }
    }
}