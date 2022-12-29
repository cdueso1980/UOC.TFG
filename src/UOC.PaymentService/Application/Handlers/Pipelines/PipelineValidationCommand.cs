using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using UOC.PaymentService.Validators;
using MediatR;

namespace UOC.PaymentService.Handlers.Pipelines
{
    public class PipelineValidationCommand<TCommand, TResult> : IPipelineBehavior<TCommand, TResult> 
    {
        private readonly IEnumerable<IValidator<TCommand>> validators;

        public PipelineValidationCommand(IEnumerable<IValidator<TCommand>> validators)
        {
            this.validators = validators ?? throw new ArgumentNullException(nameof(validators));
        }
        public async Task<TResult> Handle(TCommand request, CancellationToken cancellationToken, RequestHandlerDelegate<TResult> next)
        {
            var validationResult = await Task.WhenAll(this.validators.Select(r => r.ValidateAsync(request)));
            var fails = validationResult.Where(r => !r.IsValid).SelectMany(r => r.Errors);
            if (fails.Any())
            {
                throw new CommandValidationException(fails.Select(error => $"- Message: {error.ErrorMessage} - Property: {error.PropertyName}")
                    .Aggregate((a,b) => $"{a}{Environment.NewLine}{b}"));
            }
            return await next().ConfigureAwait(false);
        }
    }
}