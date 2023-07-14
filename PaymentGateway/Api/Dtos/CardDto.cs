using PaymentGateway.Domain.Concrete;
using PaymentGateway.Domain.Models;

namespace PaymentGateway.Api.Dtos
{
    public class MaskedCardDto
    {
        public MaskedCardDto(Card card)
        {
            Number = Obfuscator.Obfuscate(card.Number);
            CCV = Obfuscator.Obfuscate(card.CVV);
            ExpirationYear = Obfuscator.Obfuscate(card.ExpirationYear.ToString());
            ExpirationMonth = Obfuscator.Obfuscate(card.ExpirationMonth.ToString());
        }

        public string Number { get; set; }
        public string CCV { get; set; }
        public string ExpirationYear { get; set; }
        public string ExpirationMonth { get; set; }
    }

    public class CardDto
    {
        public string Number { get; set; }
        public string CCV { get; set; }
        public int ExpirationYear { get; set; }
        public int ExpirationMonth { get; set; }
    }
}
