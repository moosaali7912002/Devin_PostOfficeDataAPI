using System.Text.Json.Serialization;

namespace PostOfficeApi.Models.Dtos
{
    public class PostOfficeDataPayload
    {
       [JsonPropertyName("status")]
        public string? Status { get; set; }

        [JsonPropertyName("mail_item")]
        public PostOfficeDataRequest? MailItem { get; set; }
    }
}
