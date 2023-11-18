using TechnicalAnalysis.Domain.Interfaces.Utilities;

namespace TechnicalAnalysis.Domain.Utilities
{
    public sealed class Result<TSuccess, TFail> : IResult<TSuccess, TFail>
    {
        public TSuccess SuccessValue { get; }

        public TFail FailValue { get; }

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

        public static Result<TSuccess, TFail> Success(TSuccess value)
        {
            return new Result<TSuccess, TFail>(value);
        }

        public static Result<TSuccess, TFail> Fail(TFail failValue)
        {
            return new Result<TSuccess, TFail>(failValue);
        }
    }
}
