using MediatR;
using TechnicalAnalysis.CommonModels.BusinessModels;

namespace TechnicalAnalysis.Application.Mediatr.Commands
{
    public class InsertCandlesticksCommand(IEnumerable<CandlestickExtended> candlesticks) : IRequest
    {
        public IEnumerable<CandlestickExtended> Candlesticks { get; } = candlesticks;
    }
}