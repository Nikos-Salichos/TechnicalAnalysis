using MediatR;
using TechnicalAnalysis.Application.Mediatr.Commands;
using TechnicalAnalysis.Domain.Interfaces.Infrastructure;

namespace TechnicalAnalysis.Application.Mediatr.Handlers
{
    public class InsertCryptoFearAndGreedIndexHandler(IPostgreSqlRepository repository) : IRequestHandler<InsertCryptoFearAndGreedIndexCommand>
    {
        public async Task Handle(InsertCryptoFearAndGreedIndexCommand request, CancellationToken cancellationToken)
        {
            await repository.InsertCryptoFearAndGreedIndex(request.CryptoFearAndGreedDatas);
        }
    }
}
