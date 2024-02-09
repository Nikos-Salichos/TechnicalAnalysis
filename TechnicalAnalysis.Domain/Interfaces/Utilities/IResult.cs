namespace TechnicalAnalysis.Domain.Interfaces.Utilities
{
    public interface IResult<out TSuccess, out TFail>
    {
        TSuccess SuccessValue { get; }
        TFail FailValue { get; }
        bool HasError { get; }
    }
}
