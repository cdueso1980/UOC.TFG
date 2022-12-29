using System;

namespace UOC.OrderService.Domain
{
    //Note: No DomainEvent used in this POC
    public abstract class AggregateRoot
    {
        public Guid Id { get; protected set;}
    }
}