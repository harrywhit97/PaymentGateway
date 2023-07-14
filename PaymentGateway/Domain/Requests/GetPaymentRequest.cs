using FluentValidation;
using MediatR;
using PaymentGateway.Domain.Concrete;
using PaymentGateway.Domain.Models;
using PaymentGateway.Domain.Repository;

namespace PaymentGateway.Domain.Requests
{
    public class GetPaymentRequest : IRequest<Result<Payment>>
    {
        public Guid Id { get; init; }

        public GetPaymentRequest(Guid id)
        {
            Id = id;
        }
    }

    public class GetPaymentRequestValidator : AbstractValidator<GetPaymentRequest>
    {
        public GetPaymentRequestValidator()
        {
            RuleFor(x => x.Id).NotEmpty();
        }
    }

    public class GetPaymentRequestHandler : IRequestHandler<GetPaymentRequest, Result<Payment>>
    {
        private readonly IRepository<Payment> _repository;

        public GetPaymentRequestHandler(IRepository<Payment> repository)
        {
            _repository = repository;
        }

        public Task<Result<Payment>> Handle(GetPaymentRequest request, CancellationToken cancellationToken)
            => Task.FromResult(_repository.Get(request.Id));
    }
}
