using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PostOfficeApi.Models;

[Table("PostOfficeDataEvents", Schema = "pec")]
public class PostOfficeDataEvent
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Column("PostOfficeDataId")]
    public int PostOfficeDataId { get; set; }

    [Column("PO_EventDetails", TypeName = "varchar(255)")]
    public string? PO_EventDetails { get; set; }

    [Column("PO_EventCreatedAt")]
    public DateTime? PO_EventCreatedAt { get; set; }

    // Navigation
    public PostOfficeData? PostOfficeData { get; set; }
}