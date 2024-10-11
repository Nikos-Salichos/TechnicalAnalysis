using Alpaca.Markets;
using TechnicalAnalysis.Domain.Interfaces.Utilities;

namespace TechnicalAnalysis.Domain.Interfaces.Infrastructure
{
    public interface IAlpacaClient
    {
        Task<IResult<IMultiPage<IBar>, string>> GetAlpacaData(string pairName, DateTime fromDateTime, DateTime toDateTime, BarTimeFrame barTimeFrame);
    }
}
