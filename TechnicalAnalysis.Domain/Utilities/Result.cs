using TechnicalAnalysis.Domain.Interfaces.Utilities;

namespace TechnicalAnalysis.Domain.Utilities
{
    public sealed class Result<TSuccess, TFail> : IResult<TSuccess, TFail>
    {
        public TSuccess SuccessValue { get; }

        public TFail FailValue { get; }

        public bool HasError { get; }

        public bool IsSuccess { get; }

        private Result(TSuccess successValue)
        {
            IsSuccess = true;
            HasError = false;
            SuccessValue = successValue;
        }

        private Result(TFail failValue)
        {
            IsSuccess = false;
            HasError = true;
            FailValue = failValue;
        }

        public static Result<TSuccess, TFail> Success(TSuccess value) => new(value);

        public static Result<TSuccess, TFail> Fail(TFail failValue) => new(failValue);
    }
}
