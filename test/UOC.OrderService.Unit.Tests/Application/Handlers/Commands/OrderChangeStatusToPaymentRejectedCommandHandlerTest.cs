using MassTransit;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.AutoMock;
using UOC.Messages;
using UOC.OrderService.Application.Handlers.Commands;
using UOC.OrderService.Application.Messages.Commands;
using UOC.OrderService.Domain;

namespace UOC.OrderService.Unit.Testes.Application
{
    public class OrderChangeStatusToPaymentRejectedCommandHandlerTest
    {
        private readonly Mock<ILogger> logger;
        private readonly Mock<IBus> bus;
        private readonly Mock<IOrderAggregateRepository> orderAggregateRepository;
        private readonly Mock<Order> order;
        private readonly OrderChangeStatusToPaymentRejectedCommandHandler handler;

        public OrderChangeStatusToPaymentRejectedCommandHandlerTest()
        {
            var autoMocker = new AutoMocker();
            
            this.logger = autoMocker.GetMock<ILogger>();
            this.bus = autoMocker.GetMock<IBus>();
            this.orderAggregateRepository = autoMocker.GetMock<IOrderAggregateRepository>();
            this.order = autoMocker.GetMock<Order>();
            this.handler = autoMocker.CreateInstance<OrderChangeStatusToPaymentRejectedCommandHandler>();
        }

        [Fact]
        [Trait("Category", "Unit")]
        [Trait("Layer", "Application")]
        public async Task OrderChangeStatusToPaymentRejectedCommandHandler_Not_Found_OrderId()
        {
            // Arrange
            var command = new OrderChangeStatusToPaymentRejectedCommand()
            {
                OrderId = Guid.Parse("ef7259e9-95d2-4353-8d89-c48abfa723c2"),
                CorrelationId = Guid.Parse("49aeef10-bd31-4431-a1b9-03927681ec05")
            };  

            this.orderAggregateRepository.Setup(r => r.Get(It.Is<Guid>(t => t == command.OrderId))).ReturnsAsync(null as Order);


            // Act
            await handler.Handle(command, default);

            // Verify
            this.orderAggregateRepository.Verify(r => r.Get(It.Is<Guid>(t => t == command.OrderId)), Times.Once());
            this.order.Verify(r => r.ChangeStatusToPaymentRejected(), Times.Never());
            this.bus.Verify(r => r.Publish(It.IsAny<OrderStatusChangedPaymentRejectedEvent>(), It.IsAny<CancellationToken>()), Times.Never());
        }

        [Fact]
        [Trait("Category", "Unit")]
        [Trait("Layer", "Application")]
        public async Task OrderChangeStatusToPaymentRejectedCommandHandler_Ok()
        {
            // Arrange
            var orderStatusChangedPaymentRejectedEvent = null as OrderStatusChangedPaymentRejectedEvent;
            var command = new OrderChangeStatusToPaymentRejectedCommand()
            {
                OrderId = Guid.Parse("ef7259e9-95d2-4353-8d89-c48abfa723c2"),
                CorrelationId = Guid.Parse("49aeef10-bd31-4431-a1b9-03927681ec05")
            };  
            this.order.Setup(r => r.ChangeStatusToCompleted());
            this.orderAggregateRepository.Setup(r => r.Get(It.Is<Guid>(t => t == command.OrderId))).ReturnsAsync(order.Object);
            this.bus.Setup(r => r.Publish(It.IsAny<OrderStatusChangedPaymentRejectedEvent>(), It.IsAny<CancellationToken>()))
                .Callback((OrderStatusChangedPaymentRejectedEvent @event , CancellationToken cancleToken) => 
                {
                    orderStatusChangedPaymentRejectedEvent = @event;
                });

            // Act
            await handler.Handle(command, default);

            // Assert
            Assert.NotNull(orderStatusChangedPaymentRejectedEvent);
            Assert.Equal(command.OrderId, orderStatusChangedPaymentRejectedEvent.OrderId);
            Assert.Equal(command.CorrelationId, orderStatusChangedPaymentRejectedEvent.CorrelationId);

            // Verify
            this.orderAggregateRepository.Verify(r => r.Get(It.Is<Guid>(t => t == command.OrderId)), Times.Once());
            this.order.Verify(r => r.ChangeStatusToPaymentRejected(), Times.Once());
            this.bus.Verify(r => r.Publish(orderStatusChangedPaymentRejectedEvent, It.IsAny<CancellationToken>()), Times.Once());
        }
    }
}