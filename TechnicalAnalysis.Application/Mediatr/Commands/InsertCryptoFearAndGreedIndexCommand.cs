using MediatR;
using TechnicalAnalysis.Domain.Contracts.Input.CryptoAndFearIndex;

namespace TechnicalAnalysis.Application.Mediatr.Commands
{
    public class InsertCryptoFearAndGreedIndexCommand(IEnumerable<CryptoFearAndGreedData> cryptoFearAndGreedDatas) : IRequest
    {
        public IEnumerable<CryptoFearAndGreedData> CryptoFearAndGreedDatas { get; } = cryptoFearAndGreedDatas;
    }
}
