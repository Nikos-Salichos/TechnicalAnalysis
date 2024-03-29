﻿using MediatR;
using TechnicalAnalysis.CommonModels.BusinessModels;

namespace TechnicalAnalysis.Application.Mediatr.Commands.Insert
{
    public class InsertPairsCommand(IEnumerable<PairExtended> pairs) : IRequest
    {
        public IEnumerable<PairExtended> Pairs { get; } = pairs;
    }
}