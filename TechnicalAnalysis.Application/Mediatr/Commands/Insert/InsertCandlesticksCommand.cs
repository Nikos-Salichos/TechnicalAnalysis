using MediatR;
using TechnicalAnalysis.CommonModels.BusinessModels;

namespace TechnicalAnalysis.Application.Mediatr.Commands.Insert
{
    public class InsertCandlesticksCommand(List<CandlestickExtended> candlesticks) : IRequest
    {
        public List<CandlestickExtended> Candlesticks { get; } = candlesticks;
    }
}