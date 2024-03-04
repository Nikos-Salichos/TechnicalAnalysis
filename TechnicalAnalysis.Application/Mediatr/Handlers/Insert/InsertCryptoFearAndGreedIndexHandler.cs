using MediatR;
using TechnicalAnalysis.Application.Mediatr.Commands.Insert;
using TechnicalAnalysis.Domain.Interfaces.Infrastructure;

namespace TechnicalAnalysis.Application.Mediatr.Handlers.Insert
{
    public class InsertCryptoFearAndGreedIndexHandler(IPostgreSqlRepository repository) : IRequestHandler<InsertCryptoFearAndGreedIndexCommand>
    {
        public async Task Handle(InsertCryptoFearAndGreedIndexCommand request, CancellationToken cancellationToken)
        {
            await repository.InsertCryptoFearAndGreedIndex(request.CryptoFearAndGreedDatas);
        }
    }
}
