using PaymentGateway.Domain.Concrete;
using PaymentGateway.Domain.Models;

namespace PaymentGateway.Domain.Repository
{
    public interface IRepository<T> where T : BaseRecord
    {
        Result<T> Get(Guid id);
        T Insert(T record);
        T Update(T record);
    }
}