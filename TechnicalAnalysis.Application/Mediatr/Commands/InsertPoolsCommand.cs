using MediatR;
using TechnicalAnalysis.Domain.Entities;

namespace TechnicalAnalysis.Application.Mediatr.Commands
{
    public class InsertPoolsCommand : IRequest
    {
        public IEnumerable<Pool> Pools { get; }

        public InsertPoolsCommand(IEnumerable<Pool> pools)
        {
            Pools = pools;
        }
    }
}