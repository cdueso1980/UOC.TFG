using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Moq.AutoMock;
using UOC.OrderService.Application.Messages.Commands;
using UOC.OrderService.Controllers;

namespace UOC.OrderService.Unit.Testes.Infrastructure
{
    public class OrderControllerTest
    {
        private readonly Mock<IMediator> mediator;
        private readonly OrderController controller;

        public OrderControllerTest()
        {
            var autoMocker = new AutoMocker();
            this.mediator = autoMocker.GetMock<IMediator>();
            this.controller = autoMocker.CreateInstance<OrderController>();
        }

        [Fact]
        [Trait("Category", "Unit")]
        [Trait("Layer", "Infrastructure")]
        public async Task OrderControllerTest_Ok()
        {
            // Arrange
            var orderId = Guid.Parse("ef7259e9-95d2-4353-8d89-c48abfa723c2");
            var command = null as OrderChangeStatusToPaymentCommand;
            this.mediator.Setup(r => r.Send(It.IsAny<OrderChangeStatusToPaymentCommand>(), It.IsAny<CancellationToken>()))
                .Callback((IRequest<bool> cmd, CancellationToken cancellationToken) => 
                {
                    command = (OrderChangeStatusToPaymentCommand)cmd;
                })
                .ReturnsAsync(true);

            // Act
            var result = await this.controller.PaymentOrder(orderId);

            // Assert
            Assert.Equal(typeof(OkObjectResult), result.GetType());
            Assert.NotNull(command);
            Assert.Equal(orderId, command.OrderId);

            // Verify
            this.mediator.Verify(r => r.Send(It.IsAny<OrderChangeStatusToPaymentCommand>(), It.IsAny<CancellationToken>()), Times.Once());
        }
    }
}
