using System;
using MediatR;

namespace UOC.OrderService.Application.Messages.Commands
{
	public record OrderChangeStatusToCompleteCommand : IRequest
    {
		public Guid OrderId { get; init; }
		public Guid CorrelationId { get; init; }
	}
}
