using MassTransit;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.AutoMock;
using UOC.Messages;
using UOC.PaymentService.Application.Handlers.Commands;
using UOC.PaymentService.Application.Messages.Commands;
using UOC.PaymentService.Domain;

namespace UOC.PaymentService.Unit.Testes.Application
{
    public class PayOrderCommandHandlerTest
    {
        private readonly Mock<ILogger> logger;
        private readonly Mock<IBus> bus;
        private readonly Mock<IPaymentAggregateRepository> paymentAggregateRepository;
        private readonly Mock<ICustomerPaymentInfoAggregateRepository> customerPaymentInfoAggregateRepository;
        private readonly Mock<IThirdPartyPaymentService> thirdPartyPaymentService;
        private readonly Mock<CustomerPaymentInfo> customerPaymentInfo;
        private readonly PayOrderCommandHandler handler;

        public PayOrderCommandHandlerTest()
        {
            var autoMocker = new AutoMocker();
            
            this.logger = autoMocker.GetMock<ILogger>();
            this.bus = autoMocker.GetMock<IBus>();
            this.paymentAggregateRepository = autoMocker.GetMock<IPaymentAggregateRepository>();
            this.customerPaymentInfoAggregateRepository = autoMocker.GetMock<ICustomerPaymentInfoAggregateRepository>();
            this.thirdPartyPaymentService = autoMocker.GetMock<IThirdPartyPaymentService>();
            this.customerPaymentInfo = autoMocker.GetMock<CustomerPaymentInfo>();
            this.handler = autoMocker.CreateInstance<PayOrderCommandHandler>();
        }

        [Fact]
        [Trait("Category", "Unit")]
        [Trait("Layer", "Application")]
        public async Task PayOrderCommandHandler_Not_Found_OrderId()
        {
            // Arrange
            var iban = "ES8021009469106773158484";
            var command = new PayOrderCommand()
            {
                OrderId = Guid.Parse("ef7259e9-95d2-4353-8d89-c48abfa723c2"),
                CorrelationId = Guid.Parse("49aeef10-bd31-4431-a1b9-03927681ec05"),
                CustomerId =  Guid.Parse("c3b5bf02-9d4a-4f44-b62c-92f8693c26c3"),
                TotalAmount = 1800m
            };  
            this.customerPaymentInfo.SetupGet(r => r.IBAN).Returns(iban);
            this.customerPaymentInfoAggregateRepository.Setup(r => r.Get(It.Is<Guid>(t => t == command.CustomerId))).ReturnsAsync(null as CustomerPaymentInfo);


            // Act
            await handler.Handle(command, default);

            // Verify
            this.customerPaymentInfoAggregateRepository.Verify(r => r.Get(It.Is<Guid>(t => t == command.CustomerId)), Times.Once());
            this.paymentAggregateRepository.Verify(r => r.Add(It.IsAny<Payment>()), Times.Never());
            this.thirdPartyPaymentService.Verify(r => r.SendPay(It.IsAny<ThirdPartyPaymentRequest>()), Times.Never());
            this.bus.Verify(r => r.Publish(It.IsAny<PayOrderCompletedEvent>(), It.IsAny<CancellationToken>()), Times.Never());
            this.bus.Verify(r => r.Publish(It.IsAny<PayOrderRejectedEvent>(), It.IsAny<CancellationToken>()), Times.Once());
        }

        [Fact]
        [Trait("Category", "Unit")]
        [Trait("Layer", "Application")]
        public async Task PayOrderCommandHandler_Rejected()
        {
            // Arrange
            var iban = "ES8021009469106773158484";
            var thirdPartyPaymentRequest = null as ThirdPartyPaymentRequest;
            var currentPayment = null as Payment;
            var command = new PayOrderCommand()
            {
                OrderId = Guid.Parse("ef7259e9-95d2-4353-8d89-c48abfa723c2"),
                CorrelationId = Guid.Parse("49aeef10-bd31-4431-a1b9-03927681ec05"),
                CustomerId =  Guid.Parse("c3b5bf02-9d4a-4f44-b62c-92f8693c26c3"),
                TotalAmount = 1800m
            };  

            this.customerPaymentInfo.SetupGet(r => r.IBAN).Returns(iban);
            this.customerPaymentInfoAggregateRepository.Setup(r => r.Get(It.Is<Guid>(t => t == command.CustomerId))).ReturnsAsync(this.customerPaymentInfo.Object);
            this.thirdPartyPaymentService.Setup(r => r.SendPay(It.IsAny<ThirdPartyPaymentRequest>()))
                .Callback((ThirdPartyPaymentRequest request) => 
                {
                    thirdPartyPaymentRequest = request;
                })
                .ReturnsAsync(false);
            this.paymentAggregateRepository.Setup(r => r.Add(It.IsAny<Payment>()))
                .Callback((Payment payment) => 
                {
                    currentPayment = payment;
                });

            // Act
            await handler.Handle(command, default);

            // Assert
            Assert.NotNull(thirdPartyPaymentRequest);
            Assert.Equal(command.OrderId, thirdPartyPaymentRequest.OrderId);
            Assert.Equal(command.TotalAmount, thirdPartyPaymentRequest.TotalAmount);
            Assert.Equal(iban, thirdPartyPaymentRequest.IBAN);

            Assert.NotNull(currentPayment);
            Assert.Equal(PaymentStatus.Rejected, currentPayment.Status);

            // Verify
            this.customerPaymentInfoAggregateRepository.Verify(r => r.Get(It.Is<Guid>(t => t == command.CustomerId)), Times.Once());
            this.paymentAggregateRepository.Verify(r => r.Add(It.IsAny<Payment>()), Times.Once());
            this.thirdPartyPaymentService.Verify(r => r.SendPay(It.IsAny<ThirdPartyPaymentRequest>()), Times.Once());
            this.bus.Verify(r => r.Publish(It.IsAny<PayOrderCompletedEvent>(), It.IsAny<CancellationToken>()), Times.Never());
            this.bus.Verify(r => r.Publish(It.IsAny<PayOrderRejectedEvent>(), It.IsAny<CancellationToken>()), Times.Once());
        }

        [Fact]
        [Trait("Category", "Unit")]
        [Trait("Layer", "Application")]
        public async Task PayOrderCommandHandler_Completed()
        {
            // Arrange
            var iban = "ES8021009469106773158484";
            var thirdPartyPaymentRequest = null as ThirdPartyPaymentRequest;
            var currentPayment = null as Payment;
            var command = new PayOrderCommand()
            {
                OrderId = Guid.Parse("ef7259e9-95d2-4353-8d89-c48abfa723c2"),
                CorrelationId = Guid.Parse("49aeef10-bd31-4431-a1b9-03927681ec05"),
                CustomerId =  Guid.Parse("c3b5bf02-9d4a-4f44-b62c-92f8693c26c3"),
                TotalAmount = 1800m
            };  

            this.customerPaymentInfo.SetupGet(r => r.IBAN).Returns(iban);
            this.customerPaymentInfoAggregateRepository.Setup(r => r.Get(It.Is<Guid>(t => t == command.CustomerId))).ReturnsAsync(this.customerPaymentInfo.Object);
            this.thirdPartyPaymentService.Setup(r => r.SendPay(It.IsAny<ThirdPartyPaymentRequest>()))
                .Callback((ThirdPartyPaymentRequest request) => 
                {
                    thirdPartyPaymentRequest = request;
                })
                .ReturnsAsync(true);
            this.paymentAggregateRepository.Setup(r => r.Add(It.IsAny<Payment>()))
                .Callback((Payment payment) => 
                {
                    currentPayment = payment;
                });

            // Act
            await handler.Handle(command, default);

            // Assert
            Assert.NotNull(thirdPartyPaymentRequest);
            Assert.Equal(command.OrderId, thirdPartyPaymentRequest.OrderId);
            Assert.Equal(command.TotalAmount, thirdPartyPaymentRequest.TotalAmount);
            Assert.Equal(iban, thirdPartyPaymentRequest.IBAN);

            Assert.NotNull(currentPayment);
            Assert.Equal(PaymentStatus.Completed, currentPayment.Status);

            // Verify
            this.customerPaymentInfoAggregateRepository.Verify(r => r.Get(It.Is<Guid>(t => t == command.CustomerId)), Times.Once());
            this.paymentAggregateRepository.Verify(r => r.Add(It.IsAny<Payment>()), Times.Once());
            this.thirdPartyPaymentService.Verify(r => r.SendPay(It.IsAny<ThirdPartyPaymentRequest>()), Times.Once());
            this.bus.Verify(r => r.Publish(It.IsAny<PayOrderCompletedEvent>(), It.IsAny<CancellationToken>()), Times.Once());
            this.bus.Verify(r => r.Publish(It.IsAny<PayOrderRejectedEvent>(), It.IsAny<CancellationToken>()), Times.Never());
        }
    }
}