using System.ComponentModel.DataAnnotations;

namespace TechnicalAnalysis.Domain.Settings
{
    public class DatabaseSetting
    {
        [Required]
        public required string PostgreSqlTechnicalAnalysisDockerCompose { get; init; }
    }
}
