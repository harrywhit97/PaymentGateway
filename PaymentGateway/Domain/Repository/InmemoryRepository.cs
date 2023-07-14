using PaymentGateway.Domain.Concrete;
using PaymentGateway.Domain.Exceptions;
using PaymentGateway.Domain.Models;

namespace PaymentGateway.Domain.Repository
{
    public class InmemoryRepository<T> : IRepository<T> where T : BaseRecord
    {
        private readonly List<T> _list;

        public InmemoryRepository()
        {
            _list = new List<T>();
        }

        public T Insert(T record)
        {
            var now = DateTimeOffset.Now;
            record.Created = now;
            record.Modified = now;
            record.Id = Guid.NewGuid();
            _list.Add(record);
            return record;
        }

        public T Update(T record)
        {
            var current = Get(record.Id).Unwrap();

            _list.Remove(current);

            record.Modified = DateTimeOffset.Now;
            _list.Add(record);
            return record;
        }

        public IEnumerable<T> GetAll()
            => _list;

        public Result<T> Get(Guid id)
        {
            var result = _list.FirstOrDefault(x => x.Id == id);

            if (result is default(T))
                return new NotFoundException(id, typeof(T).Name);
            return result;
        }
    }
}
