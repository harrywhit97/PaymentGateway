namespace PaymentGateway.Domain.Exceptions
{
    public class ValidationException : Exception
    {
        public ValidationException()
        {
        }

        public ValidationException(IEnumerable<string> errors)
            :base($"Invalid request. Errors:\n{string.Join(", ", errors)}")
        {
        }

        public ValidationException(string message)
            : base(message)
        {
        }

        public ValidationException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
