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
        public IList<BollingerBand> BollingerBands { get; init; } = new List<BollingerBand>();
        public IList<DonchianChannel> DonchianChannels { get; init; } = new List<DonchianChannel>();
        public IList<KeltnerChannel> KeltnerChannels { get; init; } = new List<KeltnerChannel>();
        public IList<Rsi> Rsis { get; init; } = new List<Rsi>();
        public IList<Stochastic> Stochastics { get; init; } = new List<Stochastic>();
        public IList<Adx> Adxs { get; init; } = new List<Adx>();
        public IList<Aroon> Aroons { get; init; } = new List<Aroon>();
        public IList<Cci> Ccis { get; init; } = new List<Cci>();
        public IList<MovingAverage> MovingAverages { get; init; } = new List<MovingAverage>();
        public IList<Ichimoku> Ichimokus { get; init; } = new List<Ichimoku>();
        public IList<EnhancedScan> EnhancedScans { get; init; } = new List<EnhancedScan>();
        public IList<Lowest> Lowests { get; init; } = new List<Lowest>();
        public IList<Highest> Highests { get; init; } = new List<Highest>();
        public IList<DragonFlyDoji> DragonFlyDojis { get; init; } = new List<DragonFlyDoji>();
        public IList<Hammer> Hammers { get; init; } = new List<Hammer>();
        public IList<InvertedHammer> InvertedHammers { get; init; } = new List<InvertedHammer>();
        public IList<SpinningTop> SpinningTops { get; init; } = new List<SpinningTop>();
        public IList<Marubozu> Marubozus { get; init; } = new List<Marubozu>();
        public IList<Macd> Macds { get; init; } = new List<Macd>();
        public IList<PriceFunnel> PriceFunnels { get; init; } = new List<PriceFunnel>();
        public IList<TypicalPriceReversal> TypicalPriceReversals { get; init; } = new List<TypicalPriceReversal>();
        public IList<BollingerBandsFunnel> BollingerBandsFunnels { get; init; } = new List<BollingerBandsFunnel>();
        public IList<FlagNestedCandlestickBody> FlagsNestedCandlesticksBody { get; init; } = new List<FlagNestedCandlestickBody>();
        public IList<FlagNestedCandlestickRange> FlagsNestedCandlesticksRange { get; init; } = new List<FlagNestedCandlestickRange>();
        public IList<Volatility> Volatilities { get; init; } = new List<Volatility>();
        public IList<StandardDeviation> StandardDeviations { get; init; } = new List<StandardDeviation>();
        public IList<RateOfChange> RateOfChanges { get; init; } = new List<RateOfChange>();
        public IList<AverageTrueRange> AverageTrueRanges { get; init; } = new List<AverageTrueRange>();
        public IList<ResistanceBreakout> ResistanceBreakouts { get; init; } = new List<ResistanceBreakout>();
        public IList<Fractal> Fractals { get; init; } = new List<Fractal>();
        public IList<FractalLowest> FractalLowests { get; init; } = new List<FractalLowest>();
        public IList<StandardPivotPoint> StandardPivotPoints { get; init; } = new List<StandardPivotPoint>();
        public IList<AverageRange> AverageRanges { get; init; } = new List<AverageRange>();
        public IList<CloseRelativeToPivot> CloseRelativeToPivots { get; init; } = new List<CloseRelativeToPivot>();
        public IList<VixFix> VixFixes { get; init; } = new List<VixFix>();
        public IList<VerticalHorizontalFilter> VerticalHorizontalFilters { get; init; } = new List<VerticalHorizontalFilter>();
        public IList<VerticalHorizontalFilterRange> VerticalHorizontalFilterRanges { get; init; } = new List<VerticalHorizontalFilterRange>();
        public IList<OnBalanceVolume> OnBalanceVolumes { get; init; } = new List<OnBalanceVolume>();
        public IList<PsychologicalLine> PsychologicalLines { get; init; } = new List<PsychologicalLine>();
        public Trend FractalTrend { get; set; }
        public Trend PriceTrend { get; set; }
        public IDictionary<string, double> CorrelationPerPair { get; init; } = new Dictionary<string, double>();
        public long? NumberOfTrades { get; set; }
        public long TxCount { get; set; }
        public long? Liquidity { get; set; }
        public decimal? Fees { get; set; }
        public decimal? TotalValueLockedUsd { get; set; }

        public decimal? InternalBarStrength
        {
            get
            {
                if (HighPrice - LowPrice == 0)
                {
                    return null;
                }

                return (ClosePrice - LowPrice) / (HighPrice - LowPrice);
            }
        }

        public decimal? PriceReturn
        {
            get
            {
                return OpenPrice == 0
                    ? null
                    : (ClosePrice - OpenPrice) / OpenPrice;
            }
        }

        public decimal? Range
        {
            get { return HighPrice - LowPrice; }
        }

        public decimal? Body
        {
            get
            {
                return OpenPrice.HasValue && ClosePrice.HasValue
                    ? Math.Abs(OpenPrice.Value - ClosePrice.Value)
                    : null;
            }
        }

        public decimal? TypicalPrice
        {
            get { return (HighPrice + LowPrice + ClosePrice) / 3; }
        }

        public decimal? BodyToRangeRatio
        {
            get
            {
                return Range == 0
                    ? null
                    : Body / Range;
            }
        }

        public decimal? TopTwentyPercentOfRangeInPriceUnit
        {
            get { return HighPrice - (Range / 5); }
        }

        public decimal? BottomTwentyPercentOfRangeInPriceUnit
        {
            get { return LowPrice + (Range / 5); }
        }

        public decimal? MidRangeInPriceUnit
        {
            get { return (HighPrice + LowPrice) / 2; }
        }

        public decimal? TenPercentHigherThanMidRangeInPriceUnit
        {
            get { return MidRangeInPriceUnit + MidRangeInPriceUnit * 0.1m; }
        }

        public decimal? TenPercentLowerThanMidRangeInPriceUnit
        {
            get { return MidRangeInPriceUnit - MidRangeInPriceUnit * 0.1m; }
        }

        public decimal? VolumeToTVLRatio
        {
            get
            {
                return TotalValueLockedUsd == 0
                  ? null
                  : Volume / TotalValueLockedUsd;
            }
        }

        public decimal? LiquidityToTVLRatio
        {
            get
            {
                return TotalValueLockedUsd == 0
                  ? null
                  : Liquidity / TotalValueLockedUsd;
            }
        }
    }

}
