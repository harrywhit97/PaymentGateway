namespace PaymentGateway.Api.Dtos
{
    public class ProcessPaymentDto
    {
        public Guid MerchantId { get; set; }
        public CardDto Card { get; set; }
        public MoneyDto Money { get; set; }
    }
}
