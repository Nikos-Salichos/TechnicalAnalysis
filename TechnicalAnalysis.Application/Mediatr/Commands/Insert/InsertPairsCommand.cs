using MediatR;
using TechnicalAnalysis.CommonModels.BusinessModels;

namespace TechnicalAnalysis.Application.Mediatr.Commands.Insert
{
    public class InsertPairsCommand(List<PairExtended> pairs) : IRequest
    {
        public List<PairExtended> Pairs { get; } = pairs;
    }
}