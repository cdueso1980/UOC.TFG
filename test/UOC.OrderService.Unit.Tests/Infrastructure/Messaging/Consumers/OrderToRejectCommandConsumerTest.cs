using MassTransit;
using MediatR;
using Moq;
using Moq.AutoMock;
using UOC.Messages;
using UOC.OrderService.Application.Messages.Commands;
using UOC.OrderService.Infrastructure.Messaging;

namespace UOC.OrderService.Unit.Testes.Infrastructure
{
    public class OrderToRejectCommandConsumerTest
    {
        private readonly Mock<IMediator> mediator;
        private readonly Mock<ConsumeContext<OrderToRejectCommand>> consumeContext;
        private readonly OrderToRejectCommandConsumer consumer;

        public OrderToRejectCommandConsumerTest()
        {
            var autoMocker = new AutoMocker();
            this.mediator = autoMocker.GetMock<IMediator>();
            this.consumeContext = autoMocker.GetMock<ConsumeContext<OrderToRejectCommand>>();
            this.consumer = autoMocker.CreateInstance<OrderToRejectCommandConsumer>();
        }

        [Fact]
        [Trait("Category", "Unit")]
        [Trait("Layer", "Infrastructure")]
        public async Task OrderToRejectCommandConsumer_Ok()
        {
            // Arrange
            var command = null as OrderChangeStatusToPaymentRejectedCommand;
            var message = new OrderToRejectCommand()
            {
                OrderId = Guid.Parse("ef7259e9-95d2-4353-8d89-c48abfa723c2"),
                CorrelationId = Guid.Parse("d4e31334-64a0-4262-b67c-3ac967ec11b9")
            };
            this.mediator.Setup(r => r.Send(It.IsAny<OrderChangeStatusToPaymentRejectedCommand>(), It.IsAny<CancellationToken>()))
                .Callback((IRequest<MediatR.Unit> cmd, CancellationToken cancellationToken) => 
                {
                    command = (OrderChangeStatusToPaymentRejectedCommand)cmd;
                });
            this.consumeContext.SetupGet(r => r.Message).Returns(message);

            // Act
            await this.consumer.Consume(consumeContext.Object);

            // Assert
            Assert.NotNull(command);
            Assert.Equal(message.OrderId, command.OrderId);
            Assert.Equal(message.CorrelationId, command.CorrelationId);

            // Verify
            this.mediator.Verify(r => r.Send(It.IsAny<OrderChangeStatusToPaymentRejectedCommand>(), It.IsAny<CancellationToken>()), Times.Once());
        }
    }
}
