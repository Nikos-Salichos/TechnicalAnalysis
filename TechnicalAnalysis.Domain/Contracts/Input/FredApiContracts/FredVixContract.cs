using System.Text.Json.Serialization;

namespace TechnicalAnalysis.Domain.Contracts.Input.FredApiContracts
{
    public record FredVixContract(
        [property: JsonPropertyName("realtime_start")] string RealtimeStart,
        [property: JsonPropertyName("realtime_end")] string RealtimeEnd,
        [property: JsonPropertyName("observation_start")] string ObservationStart,
        [property: JsonPropertyName("observation_end")] string ObservationEnd,
        [property: JsonPropertyName("units")] string Units,
        [property: JsonPropertyName("output_type")] int OutputType,
        [property: JsonPropertyName("file_type")] string FileType,
        [property: JsonPropertyName("order_by")] string OrderBy,
        [property: JsonPropertyName("sort_order")] string SortOrder,
        [property: JsonPropertyName("count")] int Count,
        [property: JsonPropertyName("offset")] int Offset,
        [property: JsonPropertyName("limit")] int Limit,
        [property: JsonPropertyName("observations")] List<Observation> Observations
    );

    public record Observation(
        [property: JsonPropertyName("realtime_start")] string RealtimeStart,
        [property: JsonPropertyName("realtime_end")] string RealtimeEnd,
        [property: JsonPropertyName("date")] string Date,
        [property: JsonPropertyName("value")] string Value
    );
}
