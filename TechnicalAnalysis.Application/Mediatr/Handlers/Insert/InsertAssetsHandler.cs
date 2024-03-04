using MediatR;
using TechnicalAnalysis.Application.Mediatr.Commands.Insert;
using TechnicalAnalysis.Domain.Interfaces.Infrastructure;

namespace TechnicalAnalysis.Application.Mediatr.Handlers.Insert
{
    public class InsertAssetsHandler(IPostgreSqlRepository repository) : IRequestHandler<InsertAssetsCommand>
    {
        public async Task Handle(InsertAssetsCommand request, CancellationToken cancellationToken)
        {
            await repository.InsertAssetsAsync(request.Assets);
        }
    }
}
