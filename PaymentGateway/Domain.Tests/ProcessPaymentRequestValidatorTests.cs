namespace Domain.Tests
{
    public class ProcessPaymentRequestValidatorTests
    {
        [Test]
        public void ValidRequestShouldBeValid()
        {
            // Arrange
            var request = Helpers.BuildRequest();
            var validator = new ProcessPaymentRequestValidator();

            // Act
            var result = validator.Validate(request);

            // Assert
            result.IsValid.Should().BeTrue();
        }

        [TestCase(-1, false)]
        [TestCase(0, false)]
        [TestCase(1, true)]
        public void ValidRequestWithAmountShouldBe(decimal amount, bool expected)
        {
            // Arrange
            var request = Helpers.BuildRequest();
            request.Amount.Amount = amount;
            var validator = new ProcessPaymentRequestValidator();

            // Act
            var result = validator.Validate(request);

            // Assert
            result.IsValid.Should().Be(expected);

            if (!expected)
                result.Errors.Single().ErrorMessage.Should().Be("'Amount' must be greater than '0'.");
        }
    }
}
