using TechnicalAnalysis.Domain.Interfaces.Utilities;

namespace TechnicalAnalysis.Domain.Utilities
{
    public sealed class Result<TSuccess, TFail>
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

    public static class Result
    {
        // Two-type overload for full generic specification
        public static Result<TSuccess, TFail> Success<TSuccess, TFail>(TSuccess value) =>
            Result<TSuccess, TFail>.Success(value);

        public static Result<TSuccess, TFail> Fail<TSuccess, TFail>(TFail failValue) =>
            Result<TSuccess, TFail>.Fail(failValue);

        // Single-type overloads for type inference
        public static Result<TSuccess, string> Success<TSuccess>(TSuccess value) =>
            Result<TSuccess, string>.Success(value);

        public static Result<string, TFail> Fail<TFail>(TFail failValue) =>
            Result<string, TFail>.Fail(failValue);
    }
}
