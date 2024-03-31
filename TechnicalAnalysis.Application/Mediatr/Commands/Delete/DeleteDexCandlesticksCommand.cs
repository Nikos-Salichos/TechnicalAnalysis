using MediatR;

namespace TechnicalAnalysis.Application.Mediatr.Commands.Delete
{
    public class DeleteDexCandlesticksCommand(IEnumerable<long> ids) : IRequest<bool>
    {
        public IEnumerable<long> Ids { get; } = ids;
    }
}