using MediatR;

namespace TechnicalAnalysis.Application.Mediatr.Commands
{
    public class DeletePoolsCommand(IEnumerable<long> ids) : IRequest
    {
        public IEnumerable<long> Ids { get; } = ids;
    }
}