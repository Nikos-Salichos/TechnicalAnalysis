using Microsoft.Extensions.Logging;
using Moq;
using TechnicalAnalysis.Domain.Utilities;

namespace TechnicalAnalysis.Tests.UnitTests
{
    public class PollyPolicyTest
    {
        [Fact]
        public async Task CreatePolicies_ExecutesRetriesAndLogsCorrectly()
        {
            // Arrange
            var loggerMock = new Mock<ILogger<PollyPolicy>>();
            var pollyPolicy = new PollyPolicy(loggerMock.Object);
            const int retries = 3;
            var exceptionsThrown = 0;

            // Act
            var policy = pollyPolicy.CreatePolicies<int>(retries);

            // Prepare to simulate failures
            var result = await policy.ExecuteAndCaptureAsync(() =>
            {
                exceptionsThrown++;
                throw new InvalidOperationException("Test exception");
            });

            // Assert
            Assert.Equal(retries, exceptionsThrown - 1); // Excluding the final throw
            loggerMock.Verify(
                      x => x.Log(
                          It.Is<LogLevel>(logLevel => logLevel == LogLevel.Error),
                          It.IsAny<EventId>(),
                          It.Is<It.IsAnyType>((v, _) => v.ToString().Contains("Retry attempt")),
                          It.IsAny<Exception>(),
                          (Func<It.IsAnyType, Exception, string>)It.IsAny<object>()),
                      Times.Exactly(retries));
        }
    }
}
