using MediatR;

namespace TechnicalAnalysis.Application.Mediatr.Commands
{
    public class DeleteDexCandlesticksCommand : IRequest
    {
        public IEnumerable<long> Ids { get; }

        public DeleteDexCandlesticksCommand(IEnumerable<long> ids)
        {
            Ids = ids;
        }
    }
}