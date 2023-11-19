using MediatR;
using TechnicalAnalysis.CommonModels.BusinessModels;

namespace TechnicalAnalysis.Application.Mediatr.Commands
{
    public class InsertAssetsCommand(IEnumerable<Asset> assets) : IRequest
    {
        public IEnumerable<Asset> Assets { get; } = assets;
    }
}