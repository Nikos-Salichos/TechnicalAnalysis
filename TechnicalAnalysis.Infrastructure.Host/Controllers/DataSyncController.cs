using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using TechnicalAnalysis.CommonModels.ApiRequests;
using TechnicalAnalysis.CommonModels.Enums;
using TechnicalAnalysis.Domain.Interfaces.Application;

namespace TechnicalAnalysis.Infrastructure.Host.Controllers
{
    [Route("api/v1/analysis")]
    [ApiController]
    public class DataSyncController(ISyncService syncService, ILogger<DataSyncController> logger,
        IValidator<DataProviderTimeframeRequest> dataProviderTimeframeRequest) : ControllerBase
    {
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
        [EnableRateLimiting("fixed-by-ip")]
        [HttpGet("SynchronizeProviders")]
        public Task<IActionResult> SynchronizeProvidersAsync(
            [FromQuery] DataProvider dataProvider = DataProvider.All,
            [FromQuery] Timeframe timeframe = Timeframe.Daily)
        {
            var request = new DataProviderTimeframeRequest(dataProvider, timeframe);
            logger.LogInformation("DataProviderTimeframeRequest {@dataProviderTimeframeRequest}", request);

            var validationResult = dataProviderTimeframeRequest.Validate(request);
            if (!validationResult.IsValid)
            {
                logger.LogError("ValidationResult {@validationResult}", validationResult);
                var errors = validationResult.Errors.Select(e => e.ErrorMessage);
                return Task.FromResult<IActionResult>(BadRequest(errors));
            }

            return SynchronizeInternalAsync(request);

            async Task<IActionResult> SynchronizeInternalAsync(DataProviderTimeframeRequest request)
            {
                await syncService.SynchronizeProvidersAsync(request);
                return NoContent();
            }
        }
    }
}
