using PaymentGateway.Domain.Models;

namespace PaymentGateway.Api.Dtos
{
    public class PaymentDto
    {
        public Guid Id { get; set; }
        public DateTimeOffset Created { get; set; }
        public DateTimeOffset Modified { get; set; }
        public MoneyDto Amount { get; set; }
        public PaymentStatus Status { get; set; }
        public MaskedCardDto Card { get; set; }
    }
}
