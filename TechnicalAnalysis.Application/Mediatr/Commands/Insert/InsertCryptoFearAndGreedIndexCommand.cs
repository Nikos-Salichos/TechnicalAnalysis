using MediatR;
using TechnicalAnalysis.CommonModels.BusinessModels;

namespace TechnicalAnalysis.Application.Mediatr.Commands.Insert
{
    public class InsertCryptoFearAndGreedIndexCommand(List<FearAndGreedModel> fearAndGreedModels) : IRequest
    {
        public List<FearAndGreedModel> CryptoFearAndGreedDatas { get; } = fearAndGreedModels;
    }
}
