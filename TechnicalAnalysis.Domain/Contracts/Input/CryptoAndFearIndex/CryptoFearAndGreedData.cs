using System.Text.Json.Serialization;

namespace TechnicalAnalysis.Domain.Contracts.Input.CryptoAndFearIndex
{
    public class CryptoFearAndGreedData
    {
        private string _timestamp = string.Empty;

        [JsonPropertyName("value")]
        public string Value { get; init; } = string.Empty;

        [JsonPropertyName("value_classification")]
        public string ValueClassification { get; init; } = string.Empty;

        [JsonPropertyName("timestamp")]
        public string Timestamp
        {
            get => _timestamp;
            init
            {
                _timestamp = value;
                try
                {
                    if (long.TryParse(_timestamp, out long unixTimestamp))
                    {
                        // Converts the Unix timestamp to UTC DateTime and then gets only the Date part
                        TimestampAsDateTime = DateTimeOffset.FromUnixTimeSeconds(unixTimestamp).UtcDateTime.Date;
                    }
                    else
                    {
                        // Handle invalid timestamp, could also throw an exception or log an error
                        TimestampAsDateTime = DateTime.MinValue.Date;
                    }
                }
                catch (Exception)
                {
                    // Log the exception, throw it, or set a default value
                    TimestampAsDateTime = DateTime.MinValue.Date;
                }
            }
        }

        /// <summary>
        /// The DateTime representation of the Unix timestamp. This property is not serialized.
        /// </summary>
        [JsonIgnore]
        public DateTime TimestampAsDateTime { get; private set; }
    }

}
