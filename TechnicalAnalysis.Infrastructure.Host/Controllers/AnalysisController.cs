using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using TechnicalAnalysis.CommonModels.ApiRequests;
using TechnicalAnalysis.CommonModels.Enums;
using TechnicalAnalysis.Domain.Interfaces.Application;
using DataProvider = TechnicalAnalysis.CommonModels.Enums.DataProvider;

namespace TechnicalAnalysis.Infrastructure.Host.Controllers
{
    [Route("api/v1/analysis")]
    [ApiController]
    public class AnalysisController(ISyncService syncService, IAnalysisService analysisService, ILogger<AnalysisController> logger,
         IValidator<DataProviderTimeframeRequest> dataProviderTimeframeRequest) : ControllerBase
    {
        private readonly IValidator<DataProviderTimeframeRequest> _dataProviderTimeframeRequest = dataProviderTimeframeRequest;

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
        [EnableRateLimiting("fixed-by-ip")]
        [HttpGet("SynchronizeProviders")]
        public Task<IActionResult> SynchronizeProvidersAsync([FromQuery] DataProvider dataProvider, Timeframe timeframe)
        {
            var dataProviderTimeframeRequest = new DataProviderTimeframeRequest(dataProvider, timeframe);
            logger.LogInformation("Method: {SynchronizeProvidersAsync} , dataProviderTimeframeRequest {@dataProviderTimeframeRequest}",
                 nameof(SynchronizeProvidersAsync), dataProviderTimeframeRequest);

            var validationResult = _dataProviderTimeframeRequest.Validate(dataProviderTimeframeRequest);
            if (!validationResult.IsValid)
            {
                logger.LogWarning("Method: {SynchronizeProvidersAsync} , validationResult {@validationResult}",
                    nameof(SynchronizeProvidersAsync), validationResult);
                var errors = validationResult.Errors.Select(e => e.ErrorMessage);
                return Task.FromResult<IActionResult>(BadRequest(errors));
            }

            return SynchronizeInternalAsync(dataProviderTimeframeRequest);

            async Task<IActionResult> SynchronizeInternalAsync(DataProviderTimeframeRequest request)
            {
                await syncService.SynchronizeProvidersAsync(request);
                return Ok();
            }
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
        [HttpGet("PairsIndicators")]
        [EnableRateLimiting("fixed-by-ip")]
        public async Task<IActionResult> GetPairsIndicatorsAsync([FromQuery] DataProvider provider = DataProvider.All)
        {
            logger.LogInformation("Method: {GetPairsIndicatorsAsync} , request {request}", nameof(GetPairsIndicatorsAsync), provider);
            var pairs = await analysisService.GetPairsIndicatorsAsync(provider, HttpContext);
            return Ok();
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
        [HttpGet("IndicatorsByPairName")]
        [EnableRateLimiting("fixed-by-ip")]
        public async Task<IActionResult> GetIndicatorsByPairNameAsync([FromQuery] string pairName, [FromQuery] Timeframe timeframe)
        {
            logger.LogInformation("Method: {GetIndicatorsByPairNameAsync} , request {request}", nameof(GetIndicatorsByPairNameAsync), pairName);
            var pair = await analysisService.GetIndicatorsByPairNamesAsync(pairName, timeframe);
            return Ok();
        }
    }
}