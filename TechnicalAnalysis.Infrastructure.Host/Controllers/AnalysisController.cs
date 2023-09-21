using FluentValidation;
using Microsoft.AspNetCore.Mvc;
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
        [HttpPost("SynchronizeProviders")]
        public async Task<IActionResult> SynchronizeProvidersAsync([FromBody] DataProviderTimeframeRequest dataProviderTimeframeRequest)
        {
            _logger.LogInformation("Method: {SynchronizeProvidersAsync} , dataProviderTimeframeRequest {@dataProviderTimeframeRequest}",
                nameof(SynchronizeProvidersAsync), dataProviderTimeframeRequest);

            var validationResult = _dataProviderTimeframeRequest.Validate(dataProviderTimeframeRequest);
            if (!validationResult.IsValid)
            {
                _logger.LogWarning("Method: {SynchronizeProvidersAsync} , validationResult {@validationResult}",
                    nameof(SynchronizeProvidersAsync), validationResult);
                return BadRequest(validationResult.Errors.Select(e => e.ErrorMessage));
            }

            var result = await _syncService.SynchronizeProvidersAsync(dataProviderTimeframeRequest);
            return Ok(result);
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [HttpGet("PairsIndicators")]
        public async Task<IActionResult> GetPairsIndicatorsAsync([FromQuery] DataProvider provider = DataProvider.All)
        {
            _logger.LogInformation("Method: {MethodName} , request {request}", nameof(GetPairsIndicatorsAsync), provider);
            var pairs = await _analysisService.GetPairsIndicatorsAsync(provider, HttpContext);
            return Ok(pairs);
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [HttpGet("IndicatorsByPairName")]
        public async Task<IActionResult> GetIndicatorsByPairNameAsync([FromQuery] string pairName, Timeframe timeframe)
        {
            _logger.LogInformation("Method: {MethodName} , request {request}", nameof(GetIndicatorsByPairNameAsync), pairName);
            var pair = await _analysisService.GetIndicatorsByPairNamesAsync(pairName, timeframe);
            return Ok();
        }
    }
}