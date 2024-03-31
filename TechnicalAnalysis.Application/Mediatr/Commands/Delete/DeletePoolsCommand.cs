using MediatR;

namespace TechnicalAnalysis.Application.Mediatr.Commands.Delete
{
    public class DeletePoolsCommand(IEnumerable<long> ids) : IRequest<bool>
    {
        public IEnumerable<long> Ids { get; } = ids;
    }
}