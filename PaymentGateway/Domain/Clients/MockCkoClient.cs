using MediatR;
using PaymentGateway.Domain.Concrete;
using PaymentGateway.Domain.Models;

namespace PaymentGateway.Domain.Clients
{
    public class MockCkoClient : ICkoClient
    {
        public Task<Result<Unit>> ExecuteTransactionAsync(Money amount, Card card)
        {
            return Task.FromResult(new Ok<Unit>(Unit.Value) as Result<Unit>);
        }
    }
}
