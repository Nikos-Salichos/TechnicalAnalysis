using Microsoft.AspNetCore.Mvc;
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

        public AnalysisController(ISyncService syncService, IAnalysisService analysisService)
        {
            _syncService = syncService;
            _analysisService = analysisService;
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [HttpGet("SynchronizeProviders")]
        public async Task<IActionResult> SynchronizeProvidersAsync(Provider provider)
        {
            await _syncService.SynchronizeProvidersAsync(provider);
            return Ok();
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [HttpGet("PairsIndicators")]
        public async Task<IActionResult> GetPairsIndicators([FromQuery] Provider provider = Provider.All)
        {
            var pairs = await _analysisService.GetPairsIndicatorsAsync(provider);
            return Ok(pairs);
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [HttpGet("IndicatorsByPairName")]
        public async Task<IActionResult> GetIndicatorsByPairName([FromQuery] string pairName)
        {
            var pair = await _analysisService.GetIndicatorsByPairNamesAsync(pairName);
            return Ok(pair);
        }
    }
}