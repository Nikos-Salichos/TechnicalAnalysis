using MediatR;

namespace TechnicalAnalysis.Application.Mediatr.Commands.Delete
{
    public class DeletePoolsCommand(List<long> ids) : IRequest<bool>
    {
        public List<long> Ids { get; } = ids;
    }
}