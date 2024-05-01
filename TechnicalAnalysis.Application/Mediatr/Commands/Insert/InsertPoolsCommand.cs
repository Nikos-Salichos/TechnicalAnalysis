using MediatR;
using TechnicalAnalysis.Domain.Entities;

namespace TechnicalAnalysis.Application.Mediatr.Commands.Insert
{
    public class InsertPoolsCommand(List<PoolEntity> pools) : IRequest
    {
        public List<PoolEntity> Pools { get; } = pools;
    }
}