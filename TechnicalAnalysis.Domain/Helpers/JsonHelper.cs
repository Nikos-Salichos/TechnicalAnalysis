using System.Text.Json;
using System.Text.Json.Serialization;

namespace TechnicalAnalysis.Domain.Helpers
{
    public static class JsonHelper
    {
        public static readonly JsonSerializerOptions JsonSerializerOptions = new()
        {
            WriteIndented = true,
            PropertyNameCaseInsensitive = true,
            NumberHandling = JsonNumberHandling.AllowNamedFloatingPointLiterals,
            //  DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault
        };

        public static async Task SerializeToJsonArray<T>(T item, string fileName)
        {
            var wrappedJson = JsonSerializer.Serialize(item, JsonSerializerOptions);
            var wrappedJsonArray = "[" + wrappedJson + "]";
            await File.WriteAllTextAsync(fileName, wrappedJsonArray);
        }

        public static async Task SerializeToJson<T>(T item, string fileName)
        {
            await using FileStream createStream = new(fileName, FileMode.Create);
            await JsonSerializer.SerializeAsync(createStream, item, JsonSerializerOptions);
        }
    }
}
