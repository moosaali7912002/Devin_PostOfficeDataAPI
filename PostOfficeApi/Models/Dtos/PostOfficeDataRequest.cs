using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace PostOfficeApi.Models.Dtos;

public class PostOfficeDataRequest
{
    [MaxLength(255)]
    public string? customer_name { get; set; }

    [Required]
    [MaxLength(100)]
    public string tracking_no { get; set; } = null!;

    [Required]
    [MaxLength(30)]
    public string mobile_no { get; set; } = null!;

    [MaxLength(255)]
    public string? email_address { get; set; }

    [Range(0, 9999999999999.999)]
    public decimal? weight { get; set; }

    [MaxLength(100)]
    public string? mail_class { get; set; }

    [MaxLength(100)]
    public string? mail_sub_class { get; set; }

    [MaxLength(255)]
    public string? current_destination { get; set; }

    [MaxLength(255)]
    public string? current_location { get; set; }

    public string? created_at { get; set; }

    public string? updated_at { get; set; }

    [MaxLength(3)]
    public string? origin_country { get; set; }

    [MaxLength(3)]
    public string? destination_country { get; set; }

    [MaxLength(1000)]
    public string? shipping_address { get; set; }

    [MaxLength(2000)]
    public string? items { get; set; }

    [Range(0, int.MaxValue)]
    public int? pieces { get; set; }

    [Range(0, 9999999999999999.99)]
    public decimal? value { get; set; }

    [MaxLength(100)]
    public string? package_number { get; set; }


    [MaxLength(20)]
    public string? scan_time_zone { get; set; }

    [MaxLength(100)]
    public string? PackageLastStatus { get; set; }

    // New: typed events list
    [JsonPropertyName("events")]
    public List<PostOfficeDataRequestEvent>? postOfficeDataRequestEvents { get; set; }

}
