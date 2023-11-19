using MediatR;
using TechnicalAnalysis.Domain.Entities;

namespace TechnicalAnalysis.Application.Mediatr.Commands
{
    public class InsertDexCandlesticksCommand(IEnumerable<DexCandlestick> dexCandlesticks) : IRequest
    {
        public IEnumerable<DexCandlestick> DexCandlesticks { get; } = dexCandlesticks;
    }
}