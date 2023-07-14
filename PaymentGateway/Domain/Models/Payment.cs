namespace PaymentGateway.Domain.Models
{
    public class Payment : BaseRecord
    {
        public Card Card { get; set; }
        public Money Amount { get; set; }
        public PaymentStatus Status { get; set; }
        public Guid MerchantId { get; set; }
    }

    public enum PaymentStatus
    {
        Unknown,
        Pending,
        Processed,
        Failed
    }
}
