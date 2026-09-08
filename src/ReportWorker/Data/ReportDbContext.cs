using Microsoft.EntityFrameworkCore;
using ReportWorker.Models;

namespace ReportWorker.Data;

public class ReportDbContext : DbContext
{
    public ReportDbContext(DbContextOptions<ReportDbContext> options) : base(options)
    {
    }

    public DbSet<DailyReportRequest> DailyReportRequests => Set<DailyReportRequest>();
    public DbSet<DailyReportEntry> DailyReportEntries => Set<DailyReportEntry>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<DailyReportRequest>(entity =>
        {
            entity.HasKey(x => x.Id);
            entity.Property(x => x.RequestType).IsRequired().HasMaxLength(100);
        });

        modelBuilder.Entity<DailyReportEntry>(entity =>
        {
            entity.HasKey(x => x.Id);
            entity.Property(x => x.TotalRevenue).HasColumnType("decimal(18,2)");
        });
    }
}
