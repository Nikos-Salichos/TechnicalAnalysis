using MediatR;
using TechnicalAnalysis.CommonModels.BusinessModels;

namespace TechnicalAnalysis.Application.Mediatr.Commands
{
    public class InsertPairsCommand(IEnumerable<PairExtended> pairs) : IRequest
    {
        public IEnumerable<PairExtended> Pairs { get; } = pairs;
    }
}