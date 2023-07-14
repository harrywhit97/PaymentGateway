
namespace PaymentGateway.Domain.Concrete
{
    public interface IResult { }

    public abstract class Result<TOk> : IResult
    {
        public static implicit operator Result<TOk>(TOk value)
            => new Ok<TOk>(value);

        public static implicit operator Result<TOk>(Exception value)
            => new Error<TOk>(value);
    }

    public class Ok<TOk> : Result<TOk>
    {
        public Ok(TOk value) { Value = value; }

        public TOk Value { get; }
    }

    public class Error<TError> : Result<TError>
    {
        public Error(Exception value) { Value = value; }
        public Exception Value { get; }
    }

    public static class ResultExtensions
    {
        public static TResult Match<TResult, TOk>(this Result<TOk> @this, Func<TOk, TResult> success, Func<Error<TOk>, TResult> failure)
        {
            if (@this is Error<TOk> error)
                return failure(error);
            var ok = @this as Ok<TOk>;
            return success(ok.Value);
        }

        public static TOk Unwrap<TOk>(this Result<TOk> res)
            => res switch
            {
                Ok<TOk> okResult => okResult.Value,
                _ => throw ((Error<TOk>)res).Value,
            };
    }
}
