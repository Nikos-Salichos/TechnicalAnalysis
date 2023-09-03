using MediatR;
using TechnicalAnalysis.CommonModels.BusinessModels;

namespace TechnicalAnalysis.Application.Mediatr.Commands
{
    public class InsertPairsCommand : IRequest
    {
        public IEnumerable<PairExtended> Pairs { get; }

        public InsertPairsCommand(IEnumerable<PairExtended> pairs)
        {
            Pairs = pairs;
        }
    }
}