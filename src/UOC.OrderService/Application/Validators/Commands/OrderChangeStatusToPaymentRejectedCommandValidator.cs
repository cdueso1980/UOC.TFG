using FluentValidation;
using UOC.OrderService.Application.Messages.Commands;

namespace UOC.OrderService.Application.Validators.Commands
{
    public class OrderChangeStatusToPaymentRejectedCommandValidator : AbstractValidator<OrderChangeStatusToPaymentRejectedCommand>
    {
        public OrderChangeStatusToPaymentRejectedCommandValidator()
        {
            RuleFor(command => command.OrderId).NotEmpty();
            RuleFor(command => command.CorrelationId).NotEmpty();
        }
    }
}

