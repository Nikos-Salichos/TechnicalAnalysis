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
    public class AnalysisController : ControllerBase
    {
        private readonly ISyncService _syncService;
        private readonly IAnalysisService _analysisService;
        private readonly ILogger<AnalysisController> _logger;
        private readonly IValidator<DataProviderTimeframeRequest> _dataProviderTimeframeRequest;

        public AnalysisController(ISyncService syncService, IAnalysisService analysisService, ILogger<AnalysisController> logger,
             IValidator<DataProviderTimeframeRequest> dataProviderTimeframeRequest)
        {
            _syncService = syncService;
            _analysisService = analysisService;
            _logger = logger;
            _dataProviderTimeframeRequest = dataProviderTimeframeRequest;
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
        [EnableRateLimiting("fixed-by-ip")]
        [HttpGet("SynchronizeProviders")]
        public Task<IActionResult> SynchronizeProvidersAsync([FromQuery] DataProvider dataProvider, Timeframe timeframe)
        {
            var dataProviderTimeframeRequest = new DataProviderTimeframeRequest(dataProvider, timeframe);

            _logger.LogInformation("Method: {SynchronizeProvidersAsync} , dataProviderTimeframeRequest {@dataProviderTimeframeRequest}",
                 nameof(SynchronizeProvidersAsync), dataProviderTimeframeRequest);

            var validationResult = _dataProviderTimeframeRequest.Validate(dataProviderTimeframeRequest);
            if (!validationResult.IsValid)
            {
                _logger.LogWarning("Method: {SynchronizeProvidersAsync} , validationResult {@validationResult}",
                    nameof(SynchronizeProvidersAsync), validationResult);
                var errors = validationResult.Errors.Select(e => e.ErrorMessage);
                return Task.FromResult<IActionResult>(BadRequest(errors));
            }

            return SynchronizeInternalAsync(dataProviderTimeframeRequest);

            async Task<IActionResult> SynchronizeInternalAsync(DataProviderTimeframeRequest request)
            {
                await _syncService.SynchronizeProvidersAsync(request);
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
            _logger.LogInformation("Method: {GetPairsIndicatorsAsync} , request {request}", nameof(GetPairsIndicatorsAsync), provider);
            var pairs = await _analysisService.GetPairsIndicatorsAsync(provider, HttpContext);
            return Ok(pairs);
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
        [HttpGet("IndicatorsByPairName")]
        [EnableRateLimiting("fixed-by-ip")]
        public async Task<IActionResult> GetIndicatorsByPairNameAsync([FromQuery] string pairName, [FromQuery] Timeframe timeframe)
        {
            _logger.LogInformation("Method: {GetIndicatorsByPairNameAsync} , request {request}", nameof(GetIndicatorsByPairNameAsync), pairName);
            var pair = await _analysisService.GetIndicatorsByPairNamesAsync(pairName, timeframe);
            return Ok();
        }

        [HttpGet("Indicators")]
        [EnableRateLimiting("fixed-by-ip")]
        public async Task<IActionResult> GetIndicatorsByPairNameAsyn1c()
        {
            return Ok("lala");
        }
    }
}