using MediatR;
using TechnicalAnalysis.Domain.Entities;

namespace TechnicalAnalysis.Application.Mediatr.Commands
{
    public class InsertPoolsCommand(IEnumerable<Pool> pools) : IRequest
    {
        public IEnumerable<Pool> Pools { get; } = pools;
    }
}