using MediatR;
using TechnicalAnalysis.CommonModels.BusinessModels;

namespace TechnicalAnalysis.Application.Mediatr.Commands
{
    public class InsertCandlesticksCommand : IRequest
    {
        public IEnumerable<CandlestickExtended> Candlesticks { get; }

        public InsertCandlesticksCommand(IEnumerable<CandlestickExtended> candlesticks)
        {
            Candlesticks = candlesticks;
        }
    }
}