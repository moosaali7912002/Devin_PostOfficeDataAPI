using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PostOfficeApi.Models;

[Table("PostOfficeData", Schema = "pec")]
public class PostOfficeData
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Column("NID", TypeName = "varchar(7)")]
    public string? Nid { get; set; }

    [Column("PPNo", TypeName = "varchar(50)")]
    public string? PPNo { get; set; }

    [Column("WorkPermitNo", TypeName = "varchar(50)")]
    public string? WorkPermitNo { get; set; }

    [Column("PO_CustomerName", TypeName = "varchar(255)")]
    public string? PO_CustomerName { get; set; }

    [Column("PO_TrackingNo", TypeName = "varchar(100)")]
    public string PO_TrackingNo { get; set; } = null!;

    [Column("PO_MobileNo", TypeName = "varchar(30)")]
    public string PO_MobileNo { get; set; } = null!;

    [Column("PO_EmailAddress", TypeName = "varchar(255)")]
    public string? PO_EmailAddress { get; set; }

    [Column("PO_Weight", TypeName = "decimal(18,3)")]
    public decimal? PO_Weight { get; set; }

    [Column("PO_MailClass", TypeName = "varchar(100)")]
    public string? PO_MailClass { get; set; }

    [Column("PO_MailSubClass", TypeName = "varchar(100)")]
    public string? PO_MailSubClass { get; set; }

    [Column("PO_CurrentDestination", TypeName = "varchar(255)")]
    public string? PO_CurrentDestination { get; set; }

    [Column("PO_CurrentLocation", TypeName = "varchar(255)")]
    public string? PO_CurrentLocation { get; set; }

    [Column("PO_CreatedAt")]
    public DateTime? PO_CreatedAt { get; set; }

    [Column("PO_MplUpdatedAt")]
    public DateTime? PO_MplUpdatedAt { get; set; }

    [Column("PO_OriginCountryCode", TypeName = "varchar(3)")]
    public string? PO_OriginCountryCode { get; set; }

    [Column("PO_DestinationCountryCode", TypeName = "varchar(3)")]
    public string? PO_DestinationCountryCode { get; set; }

    [Column("PO_ShippingAddress", TypeName = "varchar(1000)")]
    public string? PO_ShippingAddress { get; set; }

    [Column("PO_ItemsDescription", TypeName = "varchar(2000)")]
    public string? PO_ItemsDescription { get; set; }

    [Column("PO_Pieces")]
    public int? PO_Pieces { get; set; }

    [Column("PO_Value", TypeName = "decimal(18,2)")]
    public decimal? PO_Value { get; set; }

    [Column("PO_PackageNumber", TypeName = "varchar(100)")]
    public string? PO_PackageNumber { get; set; }
    
    [Column("PO_PackageLastStatus", TypeName = "varchar(100)")]
    public string? PO_PackageLastStatus { get; set; }

    [Column("PO_ScanTimeZone", TypeName = "varchar(50)")]
    public string? PO_ScanTimeZone { get; set; }

    [Column("IncomigPayloadReceivedDateTime")]
    public DateTime? IncomigPayloadReceivedDateTime { get; set; }

    [Column("McsUpdateAt")]
    public DateTime? McsUpdateAt { get; set; }

    [Column("McsUpdatedBy")]
    public int? McsUpdatedBy { get; set; }

    [Column("ValueCurrency", TypeName = "varchar(3)")]
    public string? ValueCurrency { get; set; }

    [Column("RecordStatus")]
    public int? RecordStatus { get; set; }

    [Column("CurrentStatus")]
    public int? CurrentStatus { get; set; }

    [Column("ReleasingLocation", TypeName = "varchar(255)")]
    public string? ReleasingLocation { get; set; }

    [Column("ReleasingOfficeCode", TypeName = "varchar(4)")]
    public string? ReleasingOfficeCode { get; set; }

    [Column("ReleasedBy")]
    public int? ReleasedBy { get; set; }

    [Column("ReleasedDateTime")]
    public DateTime? ReleasedDateTime { get; set; }

    [Column("ReceiverNid", TypeName = "varchar(7)")]
    public string? ReceiverNid { get; set; }

    [Column("ReceiverPPNo", TypeName = "varchar(50)")]
    public string? ReceiverPPNo { get; set; }

    [Column("ReceiverWorkPermitNo", TypeName = "varchar(50)")]
    public string? ReceiverWorkPermitNo { get; set; }

    [Column("ReceiverMobileNo", TypeName = "varchar(30)")]
    public string? ReceiverMobileNo { get; set; }

    [Column("ReceiverAddress", TypeName = "varchar(1000)")]
    public string? ReceiverAddress { get; set; }

    [Column("IsHit")]
    public bool? IsHit { get; set; }

    // Navigation property for related events
    public List<PostOfficeDataEvent>? postOfficeDataEvents { get; set; }
}
