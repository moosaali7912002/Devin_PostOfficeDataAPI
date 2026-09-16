using System.ComponentModel.DataAnnotations;

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

    [EmailAddress]
    [MaxLength(255)]
    public string? email_address { get; set; }

    [Range(0, 9999999999999.999)]
    public decimal? weight { get; set; }

    [MaxLength(255)]
    public string? current_destination { get; set; }

    [MaxLength(255)]
    public string? current_location { get; set; }

    public DateTime? created_at { get; set; }

    public DateTime? updated_at { get; set; }

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

    [MaxLength(100)]
    public string? ServiceType { get; set; }

    [MaxLength(100)]
    public string? PackageLastStatus { get; set; }

    //[MaxLength(255)]
    //public string? PO_CustomerName { get; set; }

    //[Required]
    //[MaxLength(100)]
    //public string PO_TrackingNo { get; set; } = null!;

    //[Required]
    //[MaxLength(30)]
    //public string PO_MobileNo { get; set; } = null!;

    //[EmailAddress]
    //[MaxLength(255)]
    //public string? PO_EmailAddress { get; set; }

    //[Range(0, 9999999999999.999)]
    //public decimal? PO_Weight { get; set; }

    //[MaxLength(255)]
    //public string? PO_CurrentDestination { get; set; }

    //[MaxLength(255)]
    //public string? PO_CurrentLocation { get; set; }

    //public DateTime? PO_CreatedAt { get; set; }

    //public DateTime? PO_MplUpdatedAt { get; set; }

    //[MaxLength(3)]
    //public string? PO_OriginCountryCode { get; set; }

    //[MaxLength(3)]
    //public string? PO_DestinationCountryCode { get; set; }

    //[MaxLength(1000)]
    //public string? PO_ShippingAddress { get; set; }

    //[MaxLength(2000)]
    //public string? PO_ItemsDescription { get; set; }

    //[Range(0, int.MaxValue)]
    //public int? PO_Pieces { get; set; }

    //[Range(0, 9999999999999999.99)]
    //public decimal? PO_Value { get; set; }

    //[MaxLength(100)]
    //public string? PO_PackageNumber { get; set; }

    //[MaxLength(100)]
    //public string? PO_ServiceType { get; set; }

    //[MaxLength(100)]
    //public string? PO_PackageLastStatus { get; set; }
}
