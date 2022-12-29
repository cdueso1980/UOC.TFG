using System;
using MassTransit;

namespace UOC.Orchestrator.Domain
{
    public class OrderPaymentState : SagaStateMachineInstance
    {
        public Guid CorrelationId { get; set; }
        public Guid OrderId { get; set; }
        public string State { get; set; }
        public bool IsPaymentRejected { get; set; }
        public DateTime LastTimeUpdated { get; set; }
        public uint xmin { get; set; }
    }
}