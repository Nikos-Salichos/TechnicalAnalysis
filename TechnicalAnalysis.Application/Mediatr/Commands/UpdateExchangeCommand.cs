using MediatR;
using TechnicalAnalysis.CommonModels.BusinessModels;

namespace TechnicalAnalysis.Application.Mediatr.Commands
{
    public class UpdateExchangeCommand : IRequest
    {
        public Provider Exchange { get; }

        public UpdateExchangeCommand(Provider exchange)
        {
            Exchange = exchange;
        }
    }
}