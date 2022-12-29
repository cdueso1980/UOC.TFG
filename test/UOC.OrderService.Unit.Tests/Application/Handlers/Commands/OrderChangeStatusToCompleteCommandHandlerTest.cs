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
    public class OrderChangeStatusToCompleteCommandHandlerTest
    {
        private readonly Mock<ILogger> logger;
        private readonly Mock<IBus> bus;
        private readonly Mock<IOrderAggregateRepository> orderAggregateRepository;
        private readonly Mock<Order> order;
        private readonly OrderChangeStatusToCompleteCommandHandler handler;

        public OrderChangeStatusToCompleteCommandHandlerTest()
        {
            var autoMocker = new AutoMocker();
            
            this.logger = autoMocker.GetMock<ILogger>();
            this.bus = autoMocker.GetMock<IBus>();
            this.orderAggregateRepository = autoMocker.GetMock<IOrderAggregateRepository>();
            this.order = autoMocker.GetMock<Order>();
            this.handler = autoMocker.CreateInstance<OrderChangeStatusToCompleteCommandHandler>();
        }

        [Fact]
        [Trait("Category", "Unit")]
        [Trait("Layer", "Application")]
        public async Task OrderChangeStatusToCompleteCommandHandler_Not_Found_OrderId()
        {
            // Arrange
            var command = new OrderChangeStatusToCompleteCommand()
            {
                OrderId = Guid.Parse("ef7259e9-95d2-4353-8d89-c48abfa723c2"),
                CorrelationId = Guid.Parse("49aeef10-bd31-4431-a1b9-03927681ec05")
            };  

            this.orderAggregateRepository.Setup(r => r.Get(It.Is<Guid>(t => t == command.OrderId))).ReturnsAsync(null as Order);


            // Act
            await handler.Handle(command, default);

            // Verify
            this.orderAggregateRepository.Verify(r => r.Get(It.Is<Guid>(t => t == command.OrderId)), Times.Once());
            this.order.Verify(r => r.ChangeStatusToCompleted(), Times.Never());
            this.bus.Verify(r => r.Publish(It.IsAny<OrderStatusChangedCompletedEvent>(), It.IsAny<CancellationToken>()), Times.Never());
        }

        [Fact]
        [Trait("Category", "Unit")]
        [Trait("Layer", "Application")]
        public async Task OrderChangeStatusToCompleteCommandHandler_Ok()
        {
            // Arrange
            var orderStatusChangedCompletedEvent = null as OrderStatusChangedCompletedEvent;
            var command = new OrderChangeStatusToCompleteCommand()
            {
                OrderId = Guid.Parse("ef7259e9-95d2-4353-8d89-c48abfa723c2"),
                CorrelationId = Guid.Parse("49aeef10-bd31-4431-a1b9-03927681ec05")
            };  
            this.order.Setup(r => r.ChangeStatusToCompleted());
            this.orderAggregateRepository.Setup(r => r.Get(It.Is<Guid>(t => t == command.OrderId))).ReturnsAsync(order.Object);
            this.bus.Setup(r => r.Publish(It.IsAny<OrderStatusChangedCompletedEvent>(), It.IsAny<CancellationToken>()))
                .Callback((OrderStatusChangedCompletedEvent @event , CancellationToken cancleToken) => 
                {
                    orderStatusChangedCompletedEvent = @event;
                });

            // Act
            await handler.Handle(command, default);

            // Assert
            Assert.NotNull(orderStatusChangedCompletedEvent);
            Assert.Equal(command.OrderId, orderStatusChangedCompletedEvent.OrderId);
            Assert.Equal(command.CorrelationId, orderStatusChangedCompletedEvent.CorrelationId);

            // Verify
            this.orderAggregateRepository.Verify(r => r.Get(It.Is<Guid>(t => t == command.OrderId)), Times.Once());
            this.order.Verify(r => r.ChangeStatusToCompleted(), Times.Once());
            this.bus.Verify(r => r.Publish(orderStatusChangedCompletedEvent, It.IsAny<CancellationToken>()), Times.Once());
        }
    }
}