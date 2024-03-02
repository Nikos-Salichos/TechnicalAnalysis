using MediatR;
using TechnicalAnalysis.Domain.Entities;

namespace TechnicalAnalysis.Application.Mediatr.Commands.Insert
{
    public class InsertDexCandlesticksCommand(IEnumerable<DexCandlestick> dexCandlesticks) : IRequest
    {
        public IEnumerable<DexCandlestick> DexCandlesticks { get; } = dexCandlesticks;
    }
}