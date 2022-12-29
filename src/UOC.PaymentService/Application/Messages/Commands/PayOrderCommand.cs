using System;
using MediatR;

namespace UOC.PaymentService.Application.Messages.Commands
{
	public record PayOrderCommand : IRequest
    {
		public Guid CorrelationId { get; init; }
		public Guid OrderId { get; init; }
		public Guid CustomerId { get; init; }
		public decimal TotalAmount { get; init; }
	}
}
