using MediatR;
using TechnicalAnalysis.Domain.Entities;

namespace TechnicalAnalysis.Application.Mediatr.Commands.Insert
{
    public class InsertPoolsCommand(IEnumerable<Pool> pools) : IRequest
    {
        public IEnumerable<Pool> Pools { get; } = pools;
    }
}