using MediatR;
using TechnicalAnalysis.Application.Mediatr.Commands.Insert;
using TechnicalAnalysis.Domain.Interfaces.Infrastructure;

namespace TechnicalAnalysis.Application.Mediatr.Handlers.Insert
{
    public class InsertPoolsHandler(IPostgreSqlRepository repository) : IRequestHandler<InsertPoolsCommand>
    {
        public async Task Handle(InsertPoolsCommand request, CancellationToken cancellationToken)
        {
            await repository.InsertPoolsAsync(request.Pools);
        }
    }
}
