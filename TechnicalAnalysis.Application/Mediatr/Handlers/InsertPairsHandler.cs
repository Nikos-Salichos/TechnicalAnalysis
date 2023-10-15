using MediatR;
using TechnicalAnalysis.Application.Mappers;
using TechnicalAnalysis.Application.Mediatr.Commands;
using TechnicalAnalysis.Domain.Interfaces.Infrastructure;

namespace TechnicalAnalysis.Application.Mediatr.Handlers
{
    public class InsertPairsHandler : IRequestHandler<InsertPairsCommand>
    {
        private readonly IPostgreSqlRepository _repository;

        public InsertPairsHandler(IPostgreSqlRepository repository)
        {
            _repository = repository;
        }

        public async Task Handle(InsertPairsCommand request, CancellationToken cancellationToken)
        {
            await _repository.InsertPairsAsync(request.Pairs.ToEntity());
        }
    }
}
