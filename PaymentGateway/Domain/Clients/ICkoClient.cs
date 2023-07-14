using MediatR;
using PaymentGateway.Domain.Concrete;
using PaymentGateway.Domain.Models;

namespace PaymentGateway.Domain.Clients
{
    public interface ICkoClient
    {
        Task<Result<Unit>> ExecuteTransactionAsync(Money amount, Card card);
    }
}