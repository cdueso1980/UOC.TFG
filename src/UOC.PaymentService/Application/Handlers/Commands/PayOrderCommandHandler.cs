using System;
using MassTransit;
using MediatR;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using UOC.PaymentService.Application.Messages.Commands;
using UOC.PaymentService.Domain;
using UOC.Messages;

namespace UOC.PaymentService.Application.Handlers.Commands
{
    public sealed class PayOrderCommandHandler : IRequestHandler<PayOrderCommand>
    {
        private readonly ILogger logger;
        private readonly IBus bus;
        private readonly IPaymentAggregateRepository paymentAggregateRepository;
        private readonly ICustomerPaymentInfoAggregateRepository customerPaymentInfoAggregateRepository;
        private readonly IThirdPartyPaymentService thirdPartyPaymentService;

        public PayOrderCommandHandler(
            ILogger<PayOrderCommandHandler> logger, 
            IBus bus, 
            IPaymentAggregateRepository paymentAggregateRepository,
            ICustomerPaymentInfoAggregateRepository customerPaymentInfoAggregateRepository,
            IThirdPartyPaymentService thirdPartyPaymentService)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.bus = bus ?? throw new ArgumentNullException(nameof(bus));
            this.paymentAggregateRepository = paymentAggregateRepository ?? throw new ArgumentNullException(nameof(paymentAggregateRepository));
            this.customerPaymentInfoAggregateRepository = customerPaymentInfoAggregateRepository ?? throw new ArgumentNullException(nameof(customerPaymentInfoAggregateRepository));
            this.thirdPartyPaymentService = thirdPartyPaymentService ?? throw new ArgumentNullException(nameof(thirdPartyPaymentService));
        }

        public async Task<Unit> Handle(PayOrderCommand request, CancellationToken cancellationToken)
        {
            var customerPaymentInfo = await this.customerPaymentInfoAggregateRepository.Get(request.CustomerId);
            if(customerPaymentInfo == null)
            {
                this.logger.LogError($"CustomerId {request.CustomerId} Not Found for OrderId:{request.OrderId}");
                await this.PublishResult(new PayOrderRejectedEvent { CorrelationId = request.CorrelationId, OrderId = request.OrderId }, cancellationToken);
                return Unit.Value;
            }

            var payment = new Payment(request.CustomerId, request.OrderId, request.TotalAmount);
            var paymentRequest = new ThirdPartyPaymentRequest() 
            {
                IBAN = customerPaymentInfo.IBAN,
                OrderId = request.OrderId,
                TotalAmount = request.TotalAmount
            };

            //Note: This call will be a candadite for another SAGA pattern!!!  
            var paymentResponse = await this.thirdPartyPaymentService.SendPay(paymentRequest);
            dynamic @event = null;

            if(paymentResponse)
            {
                payment.ChangeStatusToCompleted();
                @event = new PayOrderCompletedEvent { CorrelationId = request.CorrelationId, OrderId = request.OrderId };
            }
            else
            {
                payment.ChangeStatusToRejected();
                @event = new PayOrderRejectedEvent { CorrelationId = request.CorrelationId, OrderId = request.OrderId };
            }
            this.logger.LogInformation($"Payment with OrderId {payment.OrderId} Change Status To {payment.Status.ToString()}");
            
            await this.paymentAggregateRepository.Add(payment); 
            await this.paymentAggregateRepository.Save(); 

            // //TODO: Use Outbox pattern for this operation!!!
            await this.PublishResult(@event, cancellationToken);
            
            return Unit.Value;
        }

        private async Task PublishResult<T>(T @event, CancellationToken cancellationToken)  where T : class
        {
            await this.bus.Publish(@event, cancellationToken);
        }
    }
}