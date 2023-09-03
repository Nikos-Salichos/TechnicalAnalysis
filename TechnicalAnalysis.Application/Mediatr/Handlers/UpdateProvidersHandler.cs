using MediatR;
using TechnicalAnalysis.Application.Mediatr.Commands;
using TechnicalAnalysis.Domain.Interfaces.Infrastructure;

namespace TechnicalAnalysis.Application.Mediatr.Handlers
{
    public class UpdateProvidersHandler : IRequestHandler<UpdateExchangeCommand>
    {
        private readonly IPostgreSqlRepository _repository;

        public UpdateProvidersHandler(IPostgreSqlRepository repository)
        {
            _repository = repository;
        }

        public async Task Handle(UpdateExchangeCommand request, CancellationToken cancellationToken)
        {
            await _repository.UpdateProvider(request.Exchange);
        }
    }
}
