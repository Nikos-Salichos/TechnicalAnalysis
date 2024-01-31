using FluentAssertions;
using Microsoft.Coyote;
using Microsoft.Coyote.SystematicTesting;
using System.Runtime.CompilerServices;
using Xunit.Abstractions;

namespace TechnicalAnalysis.Tests.IntegrationTests.TestContainers
{
    public class CoyoteTest(ITestOutputHelper output)
    {
        private static void RunSystematicTest(Func<Task> test, [CallerMemberName] string? testName = null)
        {
            var configuration = Configuration.Create()
                .WithTestingIterations(1000)
                .WithVerbosityEnabled();

            var testingEngine = TestingEngine.Create(configuration, test);
            testingEngine.Run();

            if (testingEngine.TestReport.NumOfFoundBugs > 0)
            {
                var error = testingEngine.TestReport.BugReports.First();
                File.WriteAllText(testName + ".schedule", testingEngine.ReproducibleTrace);
                Assert.Fail($"Found bug: {error}");
            }
        }

        [Fact(Timeout = 5000)]
        public void Test()
        {
            RunSystematicTest(CoyoteTestMethod);
        }

        private async Task CoyoteTestMethod()
        {
            int x = 0;
            // Concurrent operations on x.
            var t1 = Task.Run(() => x = 1);
            var t2 = Task.Run(() => x = 2);

            // Join all.
            Task.WaitAll(t1, t2);

            x.Should().Be(2);
        }
    }
}
