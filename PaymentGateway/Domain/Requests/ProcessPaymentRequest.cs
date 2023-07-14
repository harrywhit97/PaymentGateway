using FluentValidation;
using MediatR;
using PaymentGateway.Domain.Clients;
using PaymentGateway.Domain.Concrete;
using PaymentGateway.Domain.Models;
using PaymentGateway.Domain.Repository;

namespace PaymentGateway.Domain.Requests
{
    public class ProcessPaymentRequest : IRequest<Result<Payment>>
    {
        public ProcessPaymentRequest(Money amount, Card card, Guid merchantId)
        {
            Amount = amount;
            Card = card;
            MerchantId = merchantId;
        }

        public Money Amount { get; init; }
        public Card Card { get; init; }
        public Guid MerchantId { get; init; }
    }

    public class ProcessPaymentRequestValidator : AbstractValidator<ProcessPaymentRequest>
    {
        public ProcessPaymentRequestValidator()
        {
            RuleFor(x => x.MerchantId).NotEmpty();
            RuleFor(x => x.Card).NotEmpty();
            RuleFor(x => x.Amount)
                .NotEmpty()
                .SetValidator(new PositiveMoneyValidator());
        }
    }

    public class PositiveMoneyValidator : AbstractValidator<Money>
    {
        public PositiveMoneyValidator()
        {
            RuleFor(x => x.Amount)
                .GreaterThan(0);
        }
    }

    public class ProcessPaymentRequestHandler : IRequestHandler<ProcessPaymentRequest, Result<Payment>>
    {
        private readonly ICkoClient _client;
        private readonly IRepository<Payment> _repository;

        public ProcessPaymentRequestHandler(ICkoClient client, IRepository<Payment> repository)
        {
            _client = client;
            _repository = repository;
        }

        public async Task<Result<Payment>> Handle(ProcessPaymentRequest request, CancellationToken cancellationToken)
        {
            var payment = CreateNewPayment(request);
            var result = await _client.ExecuteTransactionAsync(request.Amount, request.Card);

            if (result is Error<Unit> error)
            {
                payment = MarkPaymentAsFailed(payment);
                var message = $"Error processing payment id {payment.Id}: {error.Value.Message}";
                return new Exception(message);
            }
            return MarkPaymentAsProcessed(payment);
        }

        private Payment MarkPaymentAsProcessed(Payment payment)
            => MarkPaymentStatus(payment, PaymentStatus.Processed);

        private Payment MarkPaymentAsFailed(Payment payment)
            => MarkPaymentStatus(payment, PaymentStatus.Failed);

        private Payment MarkPaymentStatus(Payment payment, PaymentStatus status)
        {
            payment.Status = status;
            return _repository.Update(payment);
        }

        private Payment CreateNewPayment(ProcessPaymentRequest request)
        {
            var payment = new Payment()
            {
                Card = request.Card,
                Amount = request.Amount,
                Status = PaymentStatus.Pending
            };
            return _repository.Insert(payment);
        }
    }
}
