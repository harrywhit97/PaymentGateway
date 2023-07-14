namespace PaymentGateway.Domain.Models
{
    public abstract class BaseRecord
    {
        public Guid Id { get; set; }
        public DateTimeOffset Created { get; set; }
        public DateTimeOffset Modified { get; set; }
    }
}
