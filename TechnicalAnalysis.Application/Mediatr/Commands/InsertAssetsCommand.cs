using MediatR;
using TechnicalAnalysis.CommonModels.BusinessModels;

namespace TechnicalAnalysis.Application.Mediatr.Commands
{
    public class InsertAssetsCommand : IRequest
    {
        public IEnumerable<Asset> Assets { get; }

        public InsertAssetsCommand(IEnumerable<Asset> assets)
        {
            Assets = assets;
        }
    }
}