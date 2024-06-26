﻿using MediatR;
using TechnicalAnalysis.Application.Mediatr.Queries;
using TechnicalAnalysis.CommonModels.BusinessModels;
using TechnicalAnalysis.Domain.Interfaces.Infrastructure;

namespace TechnicalAnalysis.Application.Mediatr.Handlers.Get
{
    public class GetAssetsRankingHandler(IPostgreSqlRepository repository) : IRequestHandler<GetAssetsRankingQuery, List<AssetRanking>>
    {
        public async Task<List<AssetRanking>> Handle(GetAssetsRankingQuery getCoinPaprikaAssetsQuery, CancellationToken cancellationToken)
        {
            var result = await repository.GetCoinPaprikaAssetsAsync();
            if (result.HasError)
            {
                return [];
            }
            return result.SuccessValue;
        }
    }
}