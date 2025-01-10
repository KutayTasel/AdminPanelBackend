using FluentValidation;
using MediatR;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace GameDashboardProject.Application.Behaviors
{
    public class FluentValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
    {
        private readonly IEnumerable<IValidator<TRequest>> _validators;

        public FluentValidationBehavior(IEnumerable<IValidator<TRequest>> validators)
        {
            _validators = validators;
        }

        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            if (_validators.Any())
            {
                var context = new ValidationContext<TRequest>(request);

                var failures = _validators
               .Select(v => v.Validate(context))
               .SelectMany(result => result.Errors)
               .Where(f => f != null)
               .GroupBy(f => new { f.PropertyName, f.ErrorMessage }) 
               .Select(g => g.First()) 
               .ToList();

                if (failures.Any())
                {
                    throw new ValidationException(failures);
                }

            }

            return await next();
        }

    }
}
