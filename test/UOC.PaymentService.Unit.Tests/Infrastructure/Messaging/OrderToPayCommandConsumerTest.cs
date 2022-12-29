using MassTransit;
using MediatR;
using Moq;
using Moq.AutoMock;
using UOC.Messages;
using UOC.PaymentService.Application.Messages.Commands;
using UOC.PaymentService.Infrastructure.Messaging;

namespace UOC.PaymentService.Unit.Testes.Infrastructure
{
    public class OrderToPayCommandConsumerTest
    {
        private readonly Mock<IMediator> mediator;
        private readonly Mock<ConsumeContext<OrderToPayCommand>> consumeContext;
        private readonly OrderToPayCommandConsumer consumer;

        public OrderToPayCommandConsumerTest()
        {
            var autoMocker = new AutoMocker();
            this.mediator = autoMocker.GetMock<IMediator>();
            this.consumeContext = autoMocker.GetMock<ConsumeContext<OrderToPayCommand>>();
            this.consumer = autoMocker.CreateInstance<OrderToPayCommandConsumer>();
        }

        [Fact]
        [Trait("Category", "Unit")]
        [Trait("Layer", "Infrastructure")]
        public async Task OrderToPayCommandConsumer_Ok()
        {
            // Arrange
            var command = null as PayOrderCommand;
            var message = new OrderToPayCommand()
            {
                OrderId = Guid.Parse("ef7259e9-95d2-4353-8d89-c48abfa723c2"),
                CorrelationId = Guid.Parse("d4e31334-64a0-4262-b67c-3ac967ec11b9"),
                CustomerId = Guid.Parse("c3b5bf02-9d4a-4f44-b62c-92f8693c26c3"),
                TotalAmount = 89.87m
            };
            this.mediator.Setup(r => r.Send(It.IsAny<PayOrderCommand>(), It.IsAny<CancellationToken>()))
                .Callback((IRequest<MediatR.Unit> cmd, CancellationToken cancellationToken) => 
                {
                    command = (PayOrderCommand)cmd;
                });
            this.consumeContext.SetupGet(r => r.Message).Returns(message);

            // Act
            await this.consumer.Consume(consumeContext.Object);

            // Assert
            Assert.NotNull(command);
            Assert.Equal(message.OrderId, command.OrderId);
            Assert.Equal(message.CorrelationId, command.CorrelationId);
            Assert.Equal(message.CustomerId, command.CustomerId);
            Assert.Equal(message.TotalAmount, command.TotalAmount);

            // Verify
            this.mediator.Verify(r => r.Send(It.IsAny<PayOrderCommand>(), It.IsAny<CancellationToken>()), Times.Once());
        }
    }
}
