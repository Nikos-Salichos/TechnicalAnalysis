namespace TechnicalAnalysis.CommonModels.BusinessModels
{
    public class Position
    {
        private static int _counter;

        public long Id { get; init; }
        public DateTime OpenPositionDate { get; set; }
        public DateTime ClosePositionDate { get; set; }
        public decimal? EntryPositionPrice { get; set; }
        public decimal? ClosePositionPrice { get; set; }
        public decimal? OpenProfitAndLoss { get; set; }
        public bool OpenPosition { get; set; }
        public string? SignalType { get; set; }

        public TimeSpan DaysInPosition
            => ClosePositionDate - OpenPositionDate;

        public decimal? ClosedProfitOrLoss
            => ClosePositionPrice - EntryPositionPrice;

        public Position() => Id = Interlocked.Increment(ref _counter);
    }
}
