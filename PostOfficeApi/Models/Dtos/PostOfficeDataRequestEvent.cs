using System.Text.Json.Serialization;

namespace PostOfficeApi.Models.Dtos;

public class PostOfficeDataRequestEvent
{
    [JsonPropertyName("details")]
    public string? details { get; set; }

    [JsonPropertyName("created_at")]
    public string? created_at { get; set; }
}