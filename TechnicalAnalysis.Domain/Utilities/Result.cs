using TechnicalAnalysis.Domain.Interfaces.Utilities;

namespace TechnicalAnalysis.Domain.Utilities
{
    public sealed class Result<TSuccess, TFail> : IResult<TSuccess, TFail>
    {
        /// <summary>
        /// Gets the value representing either a successful result or a failure result.
        /// </summary>
        public TSuccess SuccessValue { get; }

        /// <summary>
        /// Gets the value representing the failure result.
        /// </summary>
        public TFail FailValue { get; }

        /// <summary>
        /// Gets a value indicating whether this result is an error (failure).
        /// </summary>
        public bool IsError { get; }

        private Result(TSuccess successValue)
        {
            IsError = false;
            SuccessValue = successValue;
        }

        private Result(TFail failValue)
        {
            IsError = true;
            FailValue = failValue;
        }

        /// <summary>
        /// Creates a new successful result instance.
        /// </summary>
        public static Result<TSuccess, TFail> Success(TSuccess value)
        {
            return new Result<TSuccess, TFail>(value);
        }

        /// <summary>
        /// Creates a new failure result instance.
        /// </summary>
        public static Result<TSuccess, TFail> Fail(TFail failValue)
        {
            return new Result<TSuccess, TFail>(failValue);
        }
    }
}
