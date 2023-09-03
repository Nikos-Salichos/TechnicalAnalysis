using MediatR;

namespace TechnicalAnalysis.Application.Mediatr.Commands
{
    public class DeletePoolsCommand : IRequest
    {
        public IEnumerable<long> Ids { get; }

        public DeletePoolsCommand(IEnumerable<long> ids)
        {
            Ids = ids;
        }
    }
}