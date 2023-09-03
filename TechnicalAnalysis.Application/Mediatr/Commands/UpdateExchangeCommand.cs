using MediatR;
using TechnicalAnalysis.CommonModels.BusinessModels;

namespace TechnicalAnalysis.Application.Mediatr.Commands
{
    public class UpdateExchangeCommand : IRequest
    {
        public Exchange Exchange { get; }

        public UpdateExchangeCommand(Exchange exchange)
        {
            Exchange = exchange;
        }
    }
}