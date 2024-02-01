using MediatR;
using TechnicalAnalysis.Application.Mediatr.Commands;
using TechnicalAnalysis.Domain.Interfaces.Infrastructure;

namespace TechnicalAnalysis.Application.Mediatr.Handlers
{
    public class InsertAssetsHandler(IPostgreSqlRepository repository) : IRequestHandler<InsertAssetsCommand>
    {
        public async Task Handle(InsertAssetsCommand request, CancellationToken cancellationToken)
        {
            await repository.InsertAssetsAsync(request.Assets);
        }
    }
}
