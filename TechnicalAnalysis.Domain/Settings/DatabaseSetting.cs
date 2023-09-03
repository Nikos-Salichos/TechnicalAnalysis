namespace TechnicalAnalysis.Domain.Settings
{
    public class DatabaseSetting
    {
        public string PostgreSqlTechnicalAnalysisLocalhost { get; init; }
        public string PostgreSqlTechnicalAnalysisDockerCompose { get; init; }
        public string RedisLocalhost { get; init; }
        public string RedisDockerCompose { get; init; }
    }
}
