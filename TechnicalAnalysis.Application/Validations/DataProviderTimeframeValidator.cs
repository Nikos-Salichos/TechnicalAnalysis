using FluentValidation;
using TechnicalAnalysis.CommonModels.ApiRequests;
using TechnicalAnalysis.CommonModels.Enums;

namespace TechnicalAnalysis.Application.Validations
{
    public class DataProviderTimeframeValidator : AbstractValidator<DataProviderTimeframeRequest>
    {
        public DataProviderTimeframeValidator()
        {
            RuleFor(x => x)
                .Custom((value, context) =>
                {
                    bool isDailyTimeframe = value.Timeframe == Timeframe.Daily;
                    bool isBinanceWithWeekly = value is { DataProvider: DataProvider.Binance, Timeframe: Timeframe.Weekly };

                    if (!isDailyTimeframe && !isBinanceWithWeekly)
                    {
                        context.AddFailure($"Combination {value.DataProvider} and {value.Timeframe} timeframe is not supported yet");
                    }
                });
        }
    }
}
