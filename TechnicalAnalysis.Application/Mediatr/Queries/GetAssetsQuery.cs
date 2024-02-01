using MediatR;
using TechnicalAnalysis.CommonModels.BusinessModels;

namespace TechnicalAnalysis.Application.Mediatr.Queries
{
    public class GetAssetsQuery : IRequest<IEnumerable<Asset>>
    {
    }
}
