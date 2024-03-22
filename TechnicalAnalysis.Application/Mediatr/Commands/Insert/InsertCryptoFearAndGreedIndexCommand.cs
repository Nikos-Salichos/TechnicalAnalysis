using MediatR;
using TechnicalAnalysis.Domain.Contracts.Input.CryptoFearAndGreedContracts;

namespace TechnicalAnalysis.Application.Mediatr.Commands.Insert
{
    public class InsertCryptoFearAndGreedIndexCommand(IEnumerable<CryptoFearAndGreedData> cryptoFearAndGreedDatas) : IRequest
    {
        public IEnumerable<CryptoFearAndGreedData> CryptoFearAndGreedDatas { get; } = cryptoFearAndGreedDatas;
    }
}
