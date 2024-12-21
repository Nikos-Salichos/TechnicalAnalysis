using Alpaca.Markets;
using TechnicalAnalysis.Domain.Interfaces.Utilities;
using TechnicalAnalysis.Domain.Utilities;

namespace TechnicalAnalysis.Domain.Interfaces.Infrastructure
{
    public interface IAlpacaClient
    {
        Task<Result<IMultiPage<IBar>, string>> GetAlpacaData(string pairName, DateTime fromDateTime, DateTime toDateTime, BarTimeFrame barTimeFrame);
    }
}
