﻿using TechnicalAnalysis.CommonModels.BaseClasses;

namespace TechnicalAnalysis.CommonModels.Indicators.Basic
{
    public class Adx : BaseIndicator
    {
        public double? PlusDi { get; init; }
        public double? MinusDi { get; init; }
        public double? Value { get; init; }
        public double? Adxr { get; init; }
        public Adx(long candlestickId) : base(candlestickId)
        {
        }
    }
}
