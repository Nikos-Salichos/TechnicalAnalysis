namespace TechnicalAnalysis.Domain.Interfaces.Utilities
{
    public interface IResult<TSuccess, TFail>
    {
        TSuccess SuccessValue { get; }
        TFail FailValue { get; }
        bool IsError { get; }
    }
}
