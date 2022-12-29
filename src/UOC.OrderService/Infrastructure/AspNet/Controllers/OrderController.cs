using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MediatR;
using UOC.OrderService.Application.Messages.Commands;
using UOC.OrderService.Infrastructure.AspNet;
using System.Diagnostics.Metrics;

namespace UOC.OrderService.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class OrderController : ControllerBase
    {
        private readonly IMediator mediator;
        private static readonly Meter paymentMeter = new("UOC.OrderService.OrderController.Payment");
        private static readonly Counter<long> paymentCounter = paymentMeter.CreateCounter<long>("PaymentCounter");
        
        public OrderController(IMediator mediator)
        {
            this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        [HttpPost(Name = "payment")]
        public async Task<IActionResult> PaymentOrder([FromBody] Guid orderId, CancellationToken cancellationToken = default)
        {
            paymentCounter.Add(1, new("OrderId", orderId), new("Time", DateTime.UtcNow));
            var result = await this.mediator.Send(new OrderChangeStatusToPaymentCommand() { OrderId = orderId, CorrelationId = Guid.NewGuid() });
            return Ok(new ApiResponseBase() { WaitForResponse = result ? true : false, Processed = result });
        }
    }
}