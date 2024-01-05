using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace TechnicalAnalysis.Domain.Utilities
{
    public static class ExecutionTimeLogger
    {
        public static void LogExecutionTime(Action methodToExecute,
            ILogger logger,
            [CallerMemberName] string methodName = "")
        {
            Stopwatch stopwatch = Stopwatch.StartNew();

            methodToExecute.Invoke();

            stopwatch.Stop();

            logger.LogInformation("Method {MethodName} took {ElapsedMilliseconds} milliseconds to execute.",
                       methodName, stopwatch.ElapsedMilliseconds);
        }

        public static T LogExecutionTime<T>(Func<T> methodToExecute,
            ILogger logger,
            [CallerMemberName] string methodName = "")
        {
            Stopwatch stopwatch = Stopwatch.StartNew();

            T result = methodToExecute.Invoke();

            stopwatch.Stop();

            logger.LogInformation("Invoked {MethodName} took {ElapsedMilliseconds} milliseconds to execute.",
                       methodName, stopwatch.ElapsedMilliseconds);

            return result;
        }
    }
}