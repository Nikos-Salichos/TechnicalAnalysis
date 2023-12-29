using System.Text.Json;
using System.Text.Json.Serialization;

namespace TechnicalAnalysis.Application.Helpers
{
    public static class JsonHelper
    {
        private static readonly JsonSerializerOptions options = new JsonSerializerOptions
        {
            NumberHandling = JsonNumberHandling.AllowNamedFloatingPointLiterals
        };

        public static async Task SerializeToJsonArray<T>(T item, string fileName)
        {
            // Wrap the serialized JSON in an array
            var wrappedJson = JsonSerializer.Serialize(item, options);
            var wrappedJsonArray = "[" + wrappedJson + "]";

            // Write the wrapped JSON array to the file
            await File.WriteAllTextAsync(fileName, wrappedJsonArray);
        }

        public static async Task SerializeToJson<T>(T item, string fileName)
        {
            await using FileStream createStream = new(fileName, FileMode.Create);
            await JsonSerializer.SerializeAsync(createStream, item, options);
        }
    }
}
