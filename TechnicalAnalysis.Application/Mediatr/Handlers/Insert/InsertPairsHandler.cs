﻿using MediatR;
using TechnicalAnalysis.Application.Mappers;
using TechnicalAnalysis.Application.Mediatr.Commands.Insert;
using TechnicalAnalysis.Domain.Interfaces.Infrastructure;

namespace TechnicalAnalysis.Application.Mediatr.Handlers.Insert
{
    public class InsertPairsHandler(IPostgreSqlRepository repository) : IRequestHandler<InsertPairsCommand>
    {
        public async Task Handle(InsertPairsCommand request, CancellationToken cancellationToken)
        {
            await repository.InsertPairsAsync(request.Pairs.ToEntity());
        }
    }
}
