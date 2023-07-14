namespace Domain.Tests
{
    public static class Helpers
    {
        public static ProcessPaymentRequest BuildRequest()
        {
            var amount = new Money()
            {
                Amount = 200,
                Currency = "GBP"
            };
            var card = new Card()
            {
                CVV = "000",
                ExpirationMonth = 1,
                ExpirationYear = 2027,
                Number = "1234567"
            };
            return new ProcessPaymentRequest(amount, card, Guid.NewGuid());
        }
    }
}
