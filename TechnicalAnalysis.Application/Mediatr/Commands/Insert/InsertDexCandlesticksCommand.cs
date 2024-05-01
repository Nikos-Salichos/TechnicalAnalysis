using MediatR;
using TechnicalAnalysis.Domain.Entities;

namespace TechnicalAnalysis.Application.Mediatr.Commands.Insert
{
    public class InsertDexCandlesticksCommand(List<DexCandlestick> dexCandlesticks) : IRequest
    {
        public List<DexCandlestick> DexCandlesticks { get; } = dexCandlesticks;
    }
}