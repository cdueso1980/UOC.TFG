using FluentValidation;
using UOC.OrderService.Application.Messages.Commands;

namespace UOC.OrderService.Application.Validators.Commands
{
    public class OrderChangeStatusToCompleteCommandValidator : AbstractValidator<OrderChangeStatusToCompleteCommand>
    {
        public OrderChangeStatusToCompleteCommandValidator()
        {
            RuleFor(command => command.OrderId).NotEmpty();
            RuleFor(command => command.CorrelationId).NotEmpty();
        }
    }
}

