using MassTransit;
using UOC.Messages;
using UOC.Orchestrator.Domain;
using UOC.Orchestrator.Application;
using Microsoft.Extensions.DependencyInjection;
using MassTransit.Testing;

namespace UOC.Orchestrator.Unit.Testes.Application
{
    public class OrderPaymentStateMachineSagaTest
    {
        [Fact]
        [Trait("Category", "Unit")]
        [Trait("Layer", "Application")]
        public async Task OrderPaymentStateMachineSaga_Completed()
        {
            await using (var provider = new ServiceCollection()
            .AddMassTransitTestHarness(cfg => 
            { 
                cfg.AddSagaStateMachine<OrderPaymentStateMachineSaga, OrderPaymentState>();
                EndpointConvention.Map<OrderToPayCommand>(new Uri($"queue:OrderPayment"));
                EndpointConvention.Map<OrderToRejectCommand>(new Uri($"queue:OrderCancel"));
                EndpointConvention.Map<OrderToCompleteCommand>(new Uri($"queue:OrderConfirm"));
            })
            .BuildServiceProvider(true))
            {
                var harness = provider.GetRequiredService<ITestHarness>();

                await harness.Start();

                var correlationId = Guid.Parse("ef7259e9-95d2-4353-8d89-c48abfa723c2");
                var orderId = Guid.Parse("ef7259e9-95d2-4353-8d89-c48abfa723c2");
                var customerId = Guid.Parse("aef57023-6835-473b-86a2-1d3261367b99");

                var orderStatusChangedPaymentEvent = new OrderStatusChangedPaymentEvent
                {
                    CorrelationId = correlationId,
                    OrderId = orderId,
                    CustomerId = customerId,
                    TotalAmount = 166.90m
                };

                await harness.Bus.Publish(orderStatusChangedPaymentEvent);

                Assert.True(await harness.Consumed.Any<OrderStatusChangedPaymentEvent>());

                var sagaHarness = harness.GetSagaStateMachineHarness<OrderPaymentStateMachineSaga, OrderPaymentState>();

                Assert.True(await sagaHarness.Consumed.Any<OrderStatusChangedPaymentEvent>());
                Assert.True(await sagaHarness.Created.Any(x => x.CorrelationId == correlationId));
                Assert.True(await harness.Sent.Any<OrderToPayCommand>());

                var orderToPayCommand = harness.Sent.Select<OrderToPayCommand>().FirstOrDefault()?.Context.Message;

                Assert.Equal(orderStatusChangedPaymentEvent.CorrelationId, orderToPayCommand.CorrelationId);
                Assert.Equal(orderStatusChangedPaymentEvent.OrderId, orderToPayCommand.OrderId);
                Assert.Equal(orderStatusChangedPaymentEvent.CustomerId, orderToPayCommand.CustomerId);
                Assert.Equal(orderStatusChangedPaymentEvent.TotalAmount, orderToPayCommand.TotalAmount);

                var instance = sagaHarness.Created.Contains(correlationId);

                Assert.Equal(nameof(sagaHarness.StateMachine.PaymentInprogress), instance.State);

                await harness.Bus.Publish(new PayOrderCompletedEvent
                {
                    CorrelationId = correlationId,
                    OrderId = orderId
                });

                Assert.True(await harness.Consumed.Any<PayOrderCompletedEvent>());

                sagaHarness = harness.GetSagaStateMachineHarness<OrderPaymentStateMachineSaga, OrderPaymentState>();

                Assert.True(await sagaHarness.Consumed.Any<PayOrderCompletedEvent>());
                Assert.True(await sagaHarness.Created.Any(x => x.CorrelationId == correlationId));
                Assert.True(await harness.Sent.Any<OrderToCompleteCommand>());
                Assert.Equal(nameof(sagaHarness.StateMachine.PaymentConfirmed), instance.State);


                await harness.Bus.Publish(new OrderStatusChangedCompletedEvent
                {
                    CorrelationId = correlationId,
                    OrderId = orderId
                });

                Assert.True(await harness.Consumed.Any<OrderStatusChangedCompletedEvent>());

                sagaHarness = harness.GetSagaStateMachineHarness<OrderPaymentStateMachineSaga, OrderPaymentState>();

                Assert.True(await sagaHarness.Consumed.Any<OrderStatusChangedCompletedEvent>());
                Assert.True(await sagaHarness.Created.Any(x => x.CorrelationId == correlationId));
                Assert.Equal(nameof(sagaHarness.StateMachine.Final), instance.State);
            }
        }

        [Fact]
        [Trait("Category", "Unit")]
        [Trait("Layer", "Application")]
        public async Task OrderPaymentStateMachineSaga_Rejected()
        {
            await using (var provider = new ServiceCollection()
            .AddMassTransitTestHarness(cfg => 
            { 
                cfg.AddSagaStateMachine<OrderPaymentStateMachineSaga, OrderPaymentState>();
                EndpointConvention.Map<OrderToPayCommand>(new Uri($"queue:OrderPayment"));
                EndpointConvention.Map<OrderToRejectCommand>(new Uri($"queue:OrderCancel"));
                EndpointConvention.Map<OrderToCompleteCommand>(new Uri($"queue:OrderConfirm"));
            })
            .BuildServiceProvider(true))
            {
                var harness = provider.GetRequiredService<ITestHarness>();

                await harness.Start();

                var correlationId = Guid.Parse("ef7259e9-95d2-4353-8d89-c48abfa723c2");
                var orderId = Guid.Parse("ef7259e9-95d2-4353-8d89-c48abfa723c2");
                var customerId = Guid.Parse("aef57023-6835-473b-86a2-1d3261367b99");

                var orderStatusChangedPaymentEvent = new OrderStatusChangedPaymentEvent
                {
                    CorrelationId = correlationId,
                    OrderId = orderId,
                    CustomerId = customerId,
                    TotalAmount = 166.90m
                };

                await harness.Bus.Publish(orderStatusChangedPaymentEvent);

                Assert.True(await harness.Consumed.Any<OrderStatusChangedPaymentEvent>());

                var sagaHarness = harness.GetSagaStateMachineHarness<OrderPaymentStateMachineSaga, OrderPaymentState>();

                Assert.True(await sagaHarness.Consumed.Any<OrderStatusChangedPaymentEvent>());
                Assert.True(await sagaHarness.Created.Any(x => x.CorrelationId == correlationId));
                Assert.True(await harness.Sent.Any<OrderToPayCommand>());

                var orderToPayCommand = harness.Sent.Select<OrderToPayCommand>().FirstOrDefault()?.Context.Message;

                Assert.Equal(orderStatusChangedPaymentEvent.CorrelationId, orderToPayCommand.CorrelationId);
                Assert.Equal(orderStatusChangedPaymentEvent.OrderId, orderToPayCommand.OrderId);
                Assert.Equal(orderStatusChangedPaymentEvent.CustomerId, orderToPayCommand.CustomerId);
                Assert.Equal(orderStatusChangedPaymentEvent.TotalAmount, orderToPayCommand.TotalAmount);

                var instance = sagaHarness.Created.Contains(correlationId);

                Assert.Equal(nameof(sagaHarness.StateMachine.PaymentInprogress), instance.State);

                await harness.Bus.Publish(new PayOrderRejectedEvent
                {
                    CorrelationId = correlationId,
                    OrderId = orderId
                });

                Assert.True(await harness.Consumed.Any<PayOrderRejectedEvent>());

                sagaHarness = harness.GetSagaStateMachineHarness<OrderPaymentStateMachineSaga, OrderPaymentState>();

                Assert.True(await sagaHarness.Consumed.Any<PayOrderRejectedEvent>());
                Assert.True(await sagaHarness.Created.Any(x => x.CorrelationId == correlationId));
                Assert.True(await harness.Sent.Any<OrderToRejectCommand>());
                Assert.Equal(nameof(sagaHarness.StateMachine.PaymentRefused), instance.State);


                await harness.Bus.Publish(new OrderStatusChangedPaymentRejectedEvent
                {
                    CorrelationId = correlationId,
                    OrderId = orderId
                });

                Assert.True(await harness.Consumed.Any<OrderStatusChangedPaymentRejectedEvent>());

                sagaHarness = harness.GetSagaStateMachineHarness<OrderPaymentStateMachineSaga, OrderPaymentState>();

                Assert.True(await sagaHarness.Consumed.Any<OrderStatusChangedPaymentRejectedEvent>());
                Assert.True(await sagaHarness.Created.Any(x => x.CorrelationId == correlationId));
                Assert.Equal(nameof(sagaHarness.StateMachine.Final), instance.State);
            }
        }
    }
}