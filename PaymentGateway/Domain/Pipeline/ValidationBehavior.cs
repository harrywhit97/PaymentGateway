using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using PaymentGateway.Domain.Concrete;

namespace PaymentGateway.Domain.Pipeline
{
    public class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> where TResponse : IResult
    {
        private readonly IEnumerable<IValidator<TRequest>> _validators;
        private readonly ILogger<ValidationBehavior<TRequest, TResponse>> _logger;

        public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators, ILogger<ValidationBehavior<TRequest, TResponse>> logger)
        {
            _validators = validators;
            _logger = logger;
        }

        public Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            var validator = _validators.FirstOrDefault(x => x is IValidator<TRequest>);
            if (validator is default(IValidator<TRequest>))
            {
                _logger.LogWarning($"There is no validator configured for request {typeof(TRequest).Name}");
                return next();
            }

            var validationResult = validator.Validate(request);
            
            if(validationResult.IsValid) 
                return next();

            var errors = validationResult.Errors.Select(x => x.ErrorMessage);
            var exception = new Exceptions.ValidationException(validationResult.Errors.Select(x => x.ErrorMessage));

            return Task.FromResult(WrapInResultIfRequired(exception));
        }

        private TResponse WrapInResultIfRequired(Exceptions.ValidationException exception)
        {
            var responseType = typeof(TResponse);
            if (responseType.GetInterfaces().All(x => x != typeof(IResult)))
                throw exception;

            var okType = GetOkType(responseType);
            var errorType = typeof(Error<>);
            var errorOfType = errorType.MakeGenericType(okType);
            return (TResponse)Activator.CreateInstance(errorOfType, exception);
        }

        private Type GetOkType(Type responseType)
        {
            if (!responseType.IsGenericType)
                throw new Exception("Not generic!");
            return responseType.GetGenericArguments()[0];
        }
    }
}
