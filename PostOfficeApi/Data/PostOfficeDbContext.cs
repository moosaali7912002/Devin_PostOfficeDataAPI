using Microsoft.EntityFrameworkCore;
using PostOfficeApi.Models;

namespace PostOfficeApi.Data;

public class PostOfficeDbContext : DbContext
{
    public PostOfficeDbContext(DbContextOptions<PostOfficeDbContext> options) : base(options)
    {
    }

    public DbSet<PostOfficeData> PostOfficeData => Set<PostOfficeData>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<PostOfficeData>(entity =>
        {
            entity.HasIndex(e => e.PO_TrackingNo).HasDatabaseName("IX_PostOfficeData_PO_TrackingNo");
            entity.HasIndex(e => e.Nid).HasDatabaseName("IX_PostOfficeData_NID").HasFilter("[NID] IS NOT NULL");
            entity.HasIndex(e => e.PPNo).HasDatabaseName("IX_PostOfficeData_PPNo").HasFilter("[PPNo] IS NOT NULL");
            entity.HasIndex(e => e.WorkPermitNo).HasDatabaseName("IX_PostOfficeData_WorkPermitNo").HasFilter("[WorkPermitNo] IS NOT NULL");
            entity.HasIndex(e => e.ReceiverNid).HasDatabaseName("IX_PostOfficeData_ReceiverNid").HasFilter("[ReceiverNid] IS NOT NULL");
            entity.HasIndex(e => e.ReceiverPPNo).HasDatabaseName("IX_PostOfficeData_ReceiverPPNo").HasFilter("[ReceiverPPNo] IS NOT NULL");
            entity.HasIndex(e => e.ReceiverWorkPermitNo).HasDatabaseName("IX_PostOfficeData_ReceiverWorkPermitNo").HasFilter("[ReceiverWorkPermitNo] IS NOT NULL");
            entity.HasIndex(e => new { e.CurrentStatus, e.IsHit }).HasDatabaseName("IX_PostOfficeData_CurrentStatus_IsHit");
            entity.HasIndex(e => e.PO_MplUpdatedAt).HasDatabaseName("IX_PostOfficeData_PO_MplUpdatedAt");
        });
    }
}
