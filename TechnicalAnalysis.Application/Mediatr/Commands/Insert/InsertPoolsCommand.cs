using MediatR;
using TechnicalAnalysis.Domain.Entities;

namespace TechnicalAnalysis.Application.Mediatr.Commands.Insert
{
    public class InsertPoolsCommand(IEnumerable<PoolEntity> pools) : IRequest
    {
        public IEnumerable<PoolEntity> Pools { get; } = pools;
    }
}