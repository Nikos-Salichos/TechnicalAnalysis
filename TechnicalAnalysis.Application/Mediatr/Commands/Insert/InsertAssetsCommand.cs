using MediatR;
using TechnicalAnalysis.CommonModels.BusinessModels;

namespace TechnicalAnalysis.Application.Mediatr.Commands.Insert
{
    public class InsertAssetsCommand(List<Asset> assets) : IRequest
    {
        public List<Asset> Assets { get; } = assets;
    }
}