using Microsoft.AspNetCore.Mvc;
using TechnicalAnalysis.CommonModels.Enums;
using TechnicalAnalysis.Domain.Interfaces.Application;
using Provider = TechnicalAnalysis.CommonModels.Enums.Provider;

namespace TechnicalAnalysis.Infrastructure.Host.Controllers
{
    [Route("api/v1/analysis")]
    [ApiController]
    public class AnalysisController : ControllerBase
    {
        private readonly ISyncService _syncService;
        private readonly IAnalysisService _analysisService;
        private readonly ILogger<AnalysisController> _logger;

        public AnalysisController(ISyncService syncService, IAnalysisService analysisService, ILogger<AnalysisController> logger)
        {
            _syncService = syncService;
            _analysisService = analysisService;
            _logger = logger;
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [HttpGet("SynchronizeProviders")]
        public async Task<IActionResult> SynchronizeProvidersAsync(Provider provider = Provider.All, Timeframe timeframe = Timeframe.Daily)
        {
            _logger.LogInformation("Method: {SynchronizeProvidersAsync} , request {request}", nameof(SynchronizeProvidersAsync), provider);
            var result = await _syncService.SynchronizeProvidersAsync(provider, timeframe);
            return Ok(result);
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [HttpGet("PairsIndicators")]
        public async Task<IActionResult> GetPairsIndicatorsAsync([FromQuery] Provider provider = Provider.All)
        {
            _logger.LogInformation("Method: {MethodName} , request {request}", nameof(GetPairsIndicatorsAsync), provider);
            var pairs = await _analysisService.GetPairsIndicatorsAsync(provider);
            return Ok(pairs);
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [HttpGet("IndicatorsByPairName")]
        public async Task<IActionResult> GetIndicatorsByPairNameAsync([FromQuery] string pairName)
        {
            _logger.LogInformation("Method: {MethodName} , request {request}", nameof(GetIndicatorsByPairNameAsync), pairName);
            var pair = await _analysisService.GetIndicatorsByPairNamesAsync(pairName);
            return Ok(pair);
        }
    }
}