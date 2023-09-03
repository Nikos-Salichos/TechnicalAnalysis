using Microsoft.AspNetCore.Mvc;
using TechnicalAnalysis.Application.Mappers;
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
        public async Task<IActionResult> SynchronizeProvidersAsync()
        {
            await _syncService.SynchronizeProvidersAsync();
            return Ok();
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [HttpGet("PaisIndicators")]
        public async Task<IActionResult> GetPairsIndicators([FromQuery] Provider provider = Provider.All)
        {
            var pairs = await _analysisService.GetPairsIndicatorsAsync(provider);
            var contractPairs = pairs.ToOutputContract();
            return Ok(contractPairs);
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [HttpGet("IndicatorsByPairName")]
        public async Task<IActionResult> GetIndicatorsByPairName([FromQuery] string pairName)
        {
            var pairs = await _analysisService.GetIndicatorsByPairNamesAsync(pairName);
            var contractPair = pairs.ToOutputContract();
            return Ok(contractPair);
        }
    }
}