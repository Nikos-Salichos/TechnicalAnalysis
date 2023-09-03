using MediatR;
using TechnicalAnalysis.Domain.Entities;

namespace TechnicalAnalysis.Application.Mediatr.Commands
{
    public class InsertDexCandlesticksCommand : IRequest
    {
        public IEnumerable<DexCandlestick> DexCandlesticks { get; }

        public InsertDexCandlesticksCommand(IEnumerable<DexCandlestick> dexCandlesticks)
        {
            DexCandlesticks = dexCandlesticks;
        }
    }
}