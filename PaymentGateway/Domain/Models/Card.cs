namespace PaymentGateway.Domain.Models
{
    public class Card
    {
        public string Number { get; set; } = string.Empty;
        public string CVV { get; set; } = string.Empty;
        public int ExpirationYear { get; set; }
        public int ExpirationMonth { get; set; }
    }
}
