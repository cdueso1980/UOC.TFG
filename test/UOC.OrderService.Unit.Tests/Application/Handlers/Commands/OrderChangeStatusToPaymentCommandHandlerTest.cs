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
    public class OrderChangeStatusToPaymentCommandHandlerTest
    {
        private readonly Mock<ILogger> logger;
        private readonly Mock<IBus> bus;
        private readonly Mock<IOrderAggregateRepository> orderAggregateRepository;
        private readonly Mock<Order> order;
        private readonly OrderChangeStatusToPaymentCommandHandler handler;

        public OrderChangeStatusToPaymentCommandHandlerTest()
        {
            var autoMocker = new AutoMocker();
            
            this.logger = autoMocker.GetMock<ILogger>();
            this.bus = autoMocker.GetMock<IBus>();
            this.orderAggregateRepository = autoMocker.GetMock<IOrderAggregateRepository>();
            this.order = autoMocker.GetMock<Order>();
            this.handler = autoMocker.CreateInstance<OrderChangeStatusToPaymentCommandHandler>();
        }

        [Fact]
        [Trait("Category", "Unit")]
        [Trait("Layer", "Application")]
        public async Task OrderChangeStatusToPaymentCommandHandler_Not_Found_OrderId()
        {
            // Arrange
            var command = new OrderChangeStatusToPaymentCommand()
            {
                OrderId = Guid.Parse("ef7259e9-95d2-4353-8d89-c48abfa723c2"),
                CorrelationId = Guid.Parse("49aeef10-bd31-4431-a1b9-03927681ec05")
            };  

            this.orderAggregateRepository.Setup(r => r.Get(It.Is<Guid>(t => t == command.OrderId))).ReturnsAsync(null as Order);

            // Act
            var result = await handler.Handle(command, default);

            // Assert
            Assert.False(result);

            // Verify
            this.orderAggregateRepository.Verify(r => r.Get(It.Is<Guid>(t => t == command.OrderId)), Times.Once());
            this.order.Verify(r => r.ChangeStatusToPayment(), Times.Never());
            this.bus.Verify(r => r.Publish(It.IsAny<OrderStatusChangedPaymentEvent>(), It.IsAny<CancellationToken>()), Times.Never());
        }

        [Fact]
        [Trait("Category", "Unit")]
        [Trait("Layer", "Application")]
        public async Task OrderChangeStatusToPaymentCommandHandler_Ok()
        {
            // Arrange
            var orderStatusChangedPaymentEvent = null as OrderStatusChangedPaymentEvent;
            var customerId = Guid.Parse("aef57023-6835-473b-86a2-1d3261367b99");
            var totalAmount = 15871.25m;
            var command = new OrderChangeStatusToPaymentCommand()
            {
                OrderId = Guid.Parse("ef7259e9-95d2-4353-8d89-c48abfa723c2"),
                CorrelationId = Guid.Parse("49aeef10-bd31-4431-a1b9-03927681ec05")
            };  
            this.order.Setup(r => r.ChangeStatusToPayment());
            this.order.Setup(r => r.GetTotalAmount()).Returns(totalAmount);
            this.order.SetupGet(r => r.CustomerId).Returns(customerId);
            this.orderAggregateRepository.Setup(r => r.Get(It.Is<Guid>(t => t == command.OrderId))).ReturnsAsync(order.Object);
            this.bus.Setup(r => r.Publish(It.IsAny<OrderStatusChangedPaymentEvent>(), It.IsAny<CancellationToken>()))
                .Callback((OrderStatusChangedPaymentEvent @event , CancellationToken cancleToken) => 
                {
                    orderStatusChangedPaymentEvent = @event;
                });

            // Act
            var result = await handler.Handle(command, default);

            // Assert
            Assert.True(true);
            Assert.NotNull(orderStatusChangedPaymentEvent);
            Assert.Equal(command.OrderId, orderStatusChangedPaymentEvent.OrderId);
            Assert.Equal(command.CorrelationId, orderStatusChangedPaymentEvent.CorrelationId);
            Assert.Equal(customerId, orderStatusChangedPaymentEvent.CustomerId);
            Assert.Equal(totalAmount, orderStatusChangedPaymentEvent.TotalAmount);

            // Verify
            this.orderAggregateRepository.Verify(r => r.Get(It.Is<Guid>(t => t == command.OrderId)), Times.Once());
            this.order.Verify(r => r.ChangeStatusToPayment(), Times.Once());
            this.bus.Verify(r => r.Publish(orderStatusChangedPaymentEvent, It.IsAny<CancellationToken>()), Times.Once());
        }
    }
}