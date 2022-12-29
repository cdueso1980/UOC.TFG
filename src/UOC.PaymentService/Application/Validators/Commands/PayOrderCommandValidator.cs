using FluentValidation;
using UOC.PaymentService.Application.Messages.Commands;

namespace UOC.PaymentService.Application.Validators.Commands
{
    public class PayOrderCommandValidator : AbstractValidator<PayOrderCommand>
    {
        public PayOrderCommandValidator()
        {
            RuleFor(command => command.OrderId).NotEmpty();
            RuleFor(command => command.CorrelationId).NotEmpty();
            RuleFor(command => command.CustomerId).NotEmpty();
            RuleFor(command => command.TotalAmount).NotEmpty();
        }
    }
}

