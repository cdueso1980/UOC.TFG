using System;
using MediatR;

namespace UOC.OrderService.Application.Messages.Commands
{
	public record OrderChangeStatusToPaymentCommand : IRequest<bool> 
    {
		public Guid OrderId { get; init; }
		public Guid CorrelationId { get; init; }
	}
}
