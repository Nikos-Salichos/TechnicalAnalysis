using MediatR;

namespace TechnicalAnalysis.Application.Mediatr.Commands.Delete
{
    public class DeleteDexCandlesticksCommand(List<long> ids) : IRequest<bool>
    {
        public List<long> Ids { get; } = ids;
    }
}