using TechnicalAnalysis.CommonModels.BaseClasses;
using TechnicalAnalysis.CommonModels.Enums;
using TechnicalAnalysis.CommonModels.Indicators.Advanced;
using TechnicalAnalysis.CommonModels.Indicators.Basic;
using TechnicalAnalysis.CommonModels.Indicators.CandlestickFormations;

namespace TechnicalAnalysis.CommonModels.BusinessModels
{
    public class CandlestickExtended : BaseEntity
    {
        public long PoolOrPairId { get; set; }
        public string PoolOrPairName { get; set; } = string.Empty;
        public decimal? OpenPrice { get; set; }
        public decimal? HighPrice { get; set; }
        public decimal? LowPrice { get; set; }
        public decimal? ClosePrice { get; set; }
        public decimal? Volume { get; set; }
        public Timeframe Timeframe { get; set; }
        public DateTime OpenDate { get; set; }
        public DateTime CloseDate { get; set; }
        public IList<BollingerBand> BollingerBands { get; init; } = [];
        public IList<DonchianChannel> DonchianChannels { get; init; } = [];
        public IList<KeltnerChannel> KeltnerChannels { get; init; } = [];
        public IList<Rsi> Rsis { get; init; } = [];
        public IList<Stochastic> Stochastics { get; init; } = [];
        public IList<Adx> Adxs { get; init; } = [];
        public IList<Aroon> Aroons { get; init; } = [];
        public IList<Cci> Ccis { get; init; } = [];
        public IList<MovingAverage> MovingAverages { get; init; } = [];
        public IList<Ichimoku> Ichimokus { get; init; } = [];
        public IList<EnhancedScan> EnhancedScans { get; init; } = [];
        public IList<Lowest> Lowests { get; init; } = [];
        public IList<Highest> Highests { get; init; } = [];
        public IList<DragonFlyDoji> DragonFlyDojis { get; init; } = [];
        public IList<Hammer> Hammers { get; init; } = [];
        public IList<InvertedHammer> InvertedHammers { get; init; } = [];
        public IList<SpinningTop> SpinningTops { get; init; } = [];
        public IList<Marubozu> Marubozus { get; init; } = [];
        public IList<Macd> Macds { get; init; } = [];
        public IList<PriceFunnel> PriceFunnels { get; init; } = [];
        public IList<TypicalPriceReversal> TypicalPriceReversals { get; init; } = [];
        public IList<BollingerBandsFunnel> BollingerBandsFunnels { get; init; } = [];
        public IList<FlagNestedCandlestickBody> FlagsNestedCandlesticksBody { get; init; } = [];
        public IList<FlagNestedCandlestickRange> FlagsNestedCandlesticksRange { get; init; } = [];
        public IList<Volatility> Volatilities { get; init; } = [];
        public IList<StandardDeviation> StandardDeviations { get; init; } = [];
        public IList<RateOfChange> RateOfChanges { get; init; } = [];
        public IList<AverageTrueRange> AverageTrueRanges { get; init; } = [];
        public IList<ResistanceBreakout> ResistanceBreakouts { get; init; } = [];
        public IList<Fractal> Fractals { get; init; } = [];
        public IList<FractalLowest> FractalLowests { get; init; } = [];
        public IList<StandardPivotPoint> StandardPivotPoints { get; init; } = [];
        public IList<AverageRange> AverageRanges { get; init; } = [];
        public IList<CloseRelativeToPivot> CloseRelativeToPivots { get; init; } = [];
        public IList<VixFix> VixFixes { get; init; } = [];
        public IList<VerticalHorizontalFilter> VerticalHorizontalFilters { get; init; } = [];
        public IList<VerticalHorizontalFilterRange> VerticalHorizontalFilterRanges { get; init; } = [];
        public IList<OnBalanceVolume> OnBalanceVolumes { get; init; } = [];
        public IList<PsychologicalLine> PsychologicalLines { get; init; } = [];
        public Trend FractalTrend { get; set; }
        public Trend PriceTrend { get; set; }
        public IDictionary<string, double> CorrelationPerPair { get; init; } = new Dictionary<string, double>();
        public long? NumberOfTrades { get; set; }
        public long? Liquidity { get; set; }
        public decimal? Fees { get; set; }
        public decimal? TotalValueLockedUsd { get; set; }
        public int? ConsecutiveCandlesticksBelowSma { get; set; }
        public decimal? PercentageFromAllTimeHigh { get; set; }
        public decimal? DaysFromAllTimeHigh { get; set; }

        public decimal? InternalBarStrength
            => HighPrice - LowPrice == 0
                    ? null
                    : (ClosePrice - LowPrice) / (HighPrice - LowPrice);

        public decimal? PriceReturn
            => OpenPrice == 0
                    ? null
                    : (ClosePrice - OpenPrice) / OpenPrice;

        public decimal? Range => HighPrice - LowPrice;

        public decimal? Body
            => OpenPrice.HasValue && ClosePrice.HasValue
                    ? Math.Abs(OpenPrice.Value - ClosePrice.Value)
                    : null;

        public decimal? TypicalPrice
            => (HighPrice + LowPrice + ClosePrice) / 3;

        public decimal? BodyToRangeRatio
            => Range == 0
                    ? null
                    : Body / Range;

        public decimal? TopTwentyPercentOfRangeInPriceUnit
            => HighPrice - (Range / 5);

        public decimal? BottomTwentyPercentOfRangeInPriceUnit
            => LowPrice + (Range / 5);

        public decimal? MidRangeInPriceUnit
            => (HighPrice + LowPrice) / 2;

        public decimal? TenPercentHigherThanMidRangeInPriceUnit
            => MidRangeInPriceUnit + MidRangeInPriceUnit * 0.1m;

        public decimal? TenPercentLowerThanMidRangeInPriceUnit
            => MidRangeInPriceUnit - MidRangeInPriceUnit * 0.1m;

        public decimal? VolumeToTVLRatio
            => TotalValueLockedUsd == 0
                  ? null
                  : Volume / TotalValueLockedUsd;

        public decimal? LiquidityToTVLRatio
            => TotalValueLockedUsd == 0
                  ? null
                  : Liquidity / TotalValueLockedUsd;
    }

}
