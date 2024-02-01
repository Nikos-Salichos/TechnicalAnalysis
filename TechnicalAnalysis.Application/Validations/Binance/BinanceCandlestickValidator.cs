using FluentValidation;
using TechnicalAnalysis.Domain.Contracts.Input.Binance;

namespace TechnicalAnalysis.Application.Validations.Binance;

public class BinanceCandlestickValidator : AbstractValidator<BinanceCandlestick>
{
    public BinanceCandlestickValidator()
    {
        RuleFor(candlestick => candlestick).NotNull()
            .WithMessage("Candlestick cannot be null");
        RuleFor(candlestick => candlestick.OpenTime).GreaterThanOrEqualTo(DateTime.MinValue)
            .WithMessage("Open time must be greater than or equal to DateTime.MinValue.");
        RuleFor(candlestick => candlestick.CloseTime).GreaterThanOrEqualTo(DateTime.MinValue)
            .WithMessage("Close time must be greater than or equal to DateTime.MinValue.");
        RuleFor(candlestick => candlestick.CloseTime).GreaterThanOrEqualTo(candlestick => candlestick.OpenTime)
            .WithMessage("Close time must be greater than or equal to open time.");
        RuleFor(candlestick => candlestick.OpenPrice).GreaterThanOrEqualTo(0)
            .WithMessage("Open price must be greater than or equal to 0.");
        RuleFor(candlestick => candlestick.HighPrice).GreaterThanOrEqualTo(0)
            .WithMessage("High price must be greater than or equal to 0.");
        RuleFor(candlestick => candlestick.LowPrice).GreaterThanOrEqualTo(0)
            .WithMessage("Low price must be greater than or equal to 0.");
        RuleFor(candlestick => candlestick.ClosePrice).GreaterThanOrEqualTo(0)
            .WithMessage("Close price must be greater than or equal to 0.");
        RuleFor(candlestick => candlestick.OpenPrice).LessThanOrEqualTo(candlestick => candlestick.HighPrice)
            .WithMessage("Open price must be less than or equal to high price.");
        RuleFor(candlestick => candlestick.ClosePrice).LessThanOrEqualTo(candlestick => candlestick.HighPrice)
            .WithMessage("Close price must be less than or equal to high price.");
        RuleFor(candlestick => candlestick.LowPrice).LessThanOrEqualTo(candlestick => candlestick.HighPrice)
            .WithMessage("Low price must be less than or equal to high price.");
        RuleFor(candlestick => candlestick.HighPrice - candlestick.LowPrice).GreaterThanOrEqualTo(0)
            .WithMessage("The difference between HighPrice and LowPrice must be greater than or equal to 0.");
    }
}