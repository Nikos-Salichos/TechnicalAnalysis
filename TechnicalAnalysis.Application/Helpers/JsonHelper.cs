using System.Text.Json;
using System.Text.Json.Serialization;

namespace TechnicalAnalysis.Application.Helpers
{
    public static class JsonHelper
    {
        public static async Task SerializeToJsonArray<T>(T item, string fileName)
        {
            var options = new JsonSerializerOptions
            {
                NumberHandling = JsonNumberHandling.AllowNamedFloatingPointLiterals
            };

            // Wrap the serialized JSON in an array
            var wrappedJson = JsonSerializer.Serialize(item, options);
            var wrappedJsonArray = "[" + wrappedJson + "]";

            // Write the wrapped JSON array to the file
            await File.WriteAllTextAsync(fileName, wrappedJsonArray);
        }

        public static async Task SerializeToJson<T>(T item, string fileName)
        {
            var options = new JsonSerializerOptions
            {
                NumberHandling = JsonNumberHandling.AllowNamedFloatingPointLiterals
            };

            using FileStream createStream = new FileStream(fileName, FileMode.Create);
            await JsonSerializer.SerializeAsync(createStream, item, options);
        }
    }
}
