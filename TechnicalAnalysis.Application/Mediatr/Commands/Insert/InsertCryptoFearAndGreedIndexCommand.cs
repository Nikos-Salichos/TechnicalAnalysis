using MediatR;
using TechnicalAnalysis.Domain.Contracts.Input.CryptoFearAndGreedContracts;

namespace TechnicalAnalysis.Application.Mediatr.Commands.Insert
{
    public class InsertCryptoFearAndGreedIndexCommand(List<CryptoFearAndGreedData> cryptoFearAndGreedDatas) : IRequest
    {
        public List<CryptoFearAndGreedData> CryptoFearAndGreedDatas { get; } = cryptoFearAndGreedDatas;
    }
}
