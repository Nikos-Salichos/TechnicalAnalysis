﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using TechnicalAnalysis.CommonModels.BusinessModels;
using TechnicalAnalysis.CommonModels.Enums;
using TechnicalAnalysis.Domain.Interfaces.Application;
using DataProvider = TechnicalAnalysis.CommonModels.Enums.DataProvider;

namespace TechnicalAnalysis.Infrastructure.Host.Controllers
{
    [Route("api/v1/analysis")]
    [ApiController]
    public class AnalysisController(IAnalysisService analysisService, ILogger<AnalysisController> logger) : ControllerBase
    {
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
        [HttpGet("EnhancedIndicatorPairResults")]
        [EnableRateLimiting("fixed-by-ip")]
        public async Task<IActionResult> GetEnhancedPairResultsAsync([FromQuery] DataProvider provider = DataProvider.All)
        {
            logger.LogInformation("Request {request}", provider);
            var result = await analysisService.GetEnhancedPairResultsAsync(provider, HttpContext);
            return Ok(result);
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
        [HttpGet("IndicatorsByPairName")]
        [EnableRateLimiting("fixed-by-ip")]
        public Task<IActionResult> GetIndicatorsByPairNameAsync([FromQuery] List<string> pairNames, [FromQuery] Timeframe timeframe)
        {
            logger.LogInformation("Request {PairNames} {Timeframe}", pairNames, timeframe);

            if (pairNames.Count is 0)
            {
                logger.LogError("No pair names provided by the user");
                return Task.FromResult<IActionResult>(BadRequest("No pair names provided by the user"));
            }

            return GetIndicatorsByPairNameAsync(analysisService, pairNames, timeframe);

            async Task<IActionResult> GetIndicatorsByPairNameAsync(IAnalysisService analysisService, List<string> pairNames, Timeframe timeframe)
            {
                await analysisService.GetIndicatorsByPairNamesAsync(pairNames, timeframe, HttpContext);
                return NoContent();
            }
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
        [HttpGet("LayerOneAssets")]
        [EnableRateLimiting("fixed-by-ip")]
        public async Task<IActionResult> GetLayerOneAssetsAsync()
        {
            var result = await analysisService.GetLayerOneAssetsAsync();
            return Ok(result);
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
        [HttpPost("CustomCandlesticks")]
        [EnableRateLimiting("fixed-by-ip")]
        public async Task<IActionResult> GetCustomCandlesticksAnalysisAsync([FromBody] List<CustomCandlestickData> customCandlestickData)
        {
            var result = await analysisService.GetCustomCandlesticksAnalysisAsync(customCandlestickData);
            return Ok(result);
        }
    }
}