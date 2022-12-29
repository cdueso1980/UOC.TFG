using MassTransit;
using UOC.Messages;
using UOC.Orchestrator.Domain;

namespace UOC.Orchestrator.Application
{
    public class OrderPaymentStateMachineSaga : MassTransitStateMachine<OrderPaymentState>
    {
        public Event<OrderStatusChangedPaymentEvent> OrderStatusChangedPayment { get; }
        public Event<PayOrderCompletedEvent> PayOrderCompleted { get; }
        public Event<PayOrderRejectedEvent> PayOrderRejected { get; }
        public Event<OrderStatusChangedCompletedEvent> OrderCompleted { get; }
        public Event<OrderStatusChangedPaymentRejectedEvent> OrderPaymentRejected { get; }

        public State PaymentInprogress { get; }
        public State PaymentRefused { get; }
        public State PaymentConfirmed { get; }

        public OrderPaymentStateMachineSaga()
        {
            InstanceState(x => x.State);

            Event(() => OrderStatusChangedPayment, x => x.CorrelateById(context => context.Message.OrderId));
            Event(() => PayOrderCompleted, e => e.OnMissingInstance(a => a.Discard()));
            Event(() => PayOrderRejected, e => e.OnMissingInstance(a => a.Discard()));
            Event(() => OrderCompleted, e => e.OnMissingInstance(a => a.Discard()));
            Event(() => OrderPaymentRejected, e => e.OnMissingInstance(a => a.Discard()));

            Initially
            (
                When(OrderStatusChangedPayment) 
                    .Then(x => { x.Saga.OrderId = x.Message.OrderId; })
                    .Send(context => new OrderToPayCommand 
                        { 
                            CorrelationId = context.Saga.CorrelationId, 
                            OrderId = context.Message.OrderId,
                            CustomerId = context.Message.CustomerId,
                            TotalAmount = context.Message.TotalAmount
                        })
                    .TransitionTo(PaymentInprogress)
            );

            During
            (   
                PaymentInprogress,
                Ignore(OrderStatusChangedPayment),
                Ignore(OrderCompleted),
                Ignore(OrderPaymentRejected),
                When(PayOrderCompleted)
                    .Send(context => new OrderToCompleteCommand { CorrelationId = context.Saga.CorrelationId, OrderId = context.Message.OrderId })
                    .TransitionTo(PaymentConfirmed),
                When(PayOrderRejected)
                    .Send(context => new OrderToRejectCommand { CorrelationId = context.Saga.CorrelationId, OrderId = context.Message.OrderId })
                    .TransitionTo(PaymentRefused)
            );

            During
            (
                PaymentConfirmed, 
                PaymentRefused,
                Ignore(OrderStatusChangedPayment),
                Ignore(PayOrderCompleted),
                Ignore(PayOrderRejected),
                When(OrderCompleted)
                    .Then(x => { x.Saga.IsPaymentRejected = false; })
                    .Finalize(),
                When(OrderPaymentRejected)
                    .Then(x => { x.Saga.IsPaymentRejected = true; })
                    .Finalize()
            );

            //Note: Don't want delete repository entry after SAGA finish
            //SetCompletedWhenFinalized();
        }
    }
}
