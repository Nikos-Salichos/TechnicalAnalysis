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
        public List<BollingerBand> BollingerBands { get; init; } = [];
        public List<DonchianChannel> DonchianChannels { get; init; } = [];
        public List<KeltnerChannel> KeltnerChannels { get; init; } = [];
        public List<Rsi> Rsis { get; init; } = [];
        public List<Stochastic> Stochastics { get; init; } = [];
        public List<Adx> Adxs { get; init; } = [];
        public List<Aroon> Aroons { get; init; } = [];
        public List<Cci> Ccis { get; init; } = [];
        public List<MovingAverage> MovingAverages { get; init; } = [];
        public List<Ichimoku> Ichimokus { get; init; } = [];
        public List<EnhancedScan> EnhancedScans { get; init; } = [];
        public List<Lowest> Lowests { get; init; } = [];
        public List<Highest> Highests { get; init; } = [];
        public List<DragonFlyDoji> DragonFlyDojis { get; init; } = [];
        public List<Hammer> Hammers { get; init; } = [];
        public List<InvertedHammer> InvertedHammers { get; init; } = [];
        public List<SpinningTop> SpinningTops { get; init; } = [];
        public List<Marubozu> Marubozus { get; init; } = [];
        public List<Macd> Macds { get; init; } = [];
        public List<PriceFunnel> PriceFunnels { get; init; } = [];
        public List<TypicalPriceReversal> TypicalPriceReversals { get; init; } = [];
        public List<BollingerBandsFunnel> BollingerBandsFunnels { get; init; } = [];
        public List<FlagNestedCandlestickBody> FlagsNestedCandlesticksBody { get; init; } = [];
        public List<FlagNestedCandlestickRange> FlagsNestedCandlesticksRange { get; init; } = [];
        public List<Volatility> Volatilities { get; init; } = [];
        public List<StandardDeviation> StandardDeviations { get; init; } = [];
        public List<RateOfChange> RateOfChanges { get; init; } = [];
        public List<AverageTrueRange> AverageTrueRanges { get; init; } = [];
        public List<ResistanceBreakout> ResistanceBreakouts { get; init; } = [];
        public List<Fractal> Fractals { get; init; } = [];
        public List<FractalLowest> FractalLowests { get; init; } = [];
        public List<StandardPivotPoint> StandardPivotPoints { get; init; } = [];
        public List<AverageRange> AverageRanges { get; init; } = [];
        public List<CloseRelativeToPivot> CloseRelativeToPivots { get; init; } = [];
        public List<VixFix> VixFixes { get; init; } = [];
        public List<VerticalHorizontalFilter> VerticalHorizontalFilters { get; init; } = [];
        public List<VerticalHorizontalFilterRange> VerticalHorizontalFilterRanges { get; init; } = [];
        public List<OnBalanceVolume> OnBalanceVolumes { get; init; } = [];
        public List<PsychologicalLine> PsychologicalLines { get; init; } = [];
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
