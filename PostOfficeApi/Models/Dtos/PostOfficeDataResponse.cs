namespace PostOfficeApi.Models.Dtos;

public class PostOfficeDataResponse
{
    public int Id { get; set; }
    public string? PO_CustomerName { get; set; }
    public string PO_TrackingNo { get; set; } = null!;
    public string PO_MobileNo { get; set; } = null!;
    public string? PO_EmailAddress { get; set; }
    public decimal? PO_Weight { get; set; }
    public string? PO_CurrentDestination { get; set; }
    public string? PO_CurrentLocation { get; set; }
    public DateTime? PO_CreatedAt { get; set; }
    public DateTime? PO_MplUpdatedAt { get; set; }
    public string? PO_OriginCountryCode { get; set; }
    public string? PO_DestinationCountryCode { get; set; }
    public string? PO_ShippingAddress { get; set; }
    public string? PO_ItemsDescription { get; set; }
    public int? PO_Pieces { get; set; }
    public decimal? PO_Value { get; set; }
    public string? PO_PackageNumber { get; set; }
    public string? PO_ServiceType { get; set; }
    public string? PO_PackageLastStatus { get; set; }
}
