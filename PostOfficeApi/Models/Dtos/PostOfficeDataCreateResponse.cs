using System.Text.Json.Serialization;

namespace PostOfficeApi.Models.Dtos
{
    public class PostOfficeDataCreateResponse
    {
        [JsonPropertyName("success")]
        public bool Success { get; set; }

        [JsonPropertyName("message")]
        public string Message { get; set; } = null!;

        [JsonPropertyName("trackingNo")]
        public string TrackingNo { get; set; } = null!;

        [JsonPropertyName("recordId")]
        public int RecordId { get; set; }

        [JsonPropertyName("recordedAt")]
        public DateTime RecordedAt { get; set; }
    }
}
