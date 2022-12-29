using FluentValidation;
using UOC.OrderService.Application.Messages.Commands;

namespace UOC.OrderService.Application.Validators.Commands
{
    public class OrderChangeStatusToPaymentCommandValidator : AbstractValidator<OrderChangeStatusToPaymentCommand>
    {
        public OrderChangeStatusToPaymentCommandValidator()
        {
            RuleFor(command => command.OrderId).NotEmpty();
            RuleFor(command => command.CorrelationId).NotEmpty();
        }
    }
}

