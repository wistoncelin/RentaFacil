using Microsoft.EntityFrameworkCore;
using VehicleService.Domain.Entities;

namespace VehicleService.Infrastructure.Persistence;

public class VehicleDbContext : DbContext
{
    public VehicleDbContext(DbContextOptions<VehicleDbContext> options) : base(options)
    {
    }

    public DbSet<Vehicle> Vehicles => Set<Vehicle>();
    public DbSet<VehicleReservation> Reservations => Set<VehicleReservation>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Vehicle>(entity =>
        {
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Plate).IsRequired().HasMaxLength(20);
            entity.Property(x => x.Brand).IsRequired().HasMaxLength(100);
            entity.Property(x => x.Model).IsRequired().HasMaxLength(100);
            entity.Property(x => x.DailyRate).HasColumnType("decimal(18,2)");
            entity.HasIndex(x => x.Plate).IsUnique();
        });

        modelBuilder.Entity<VehicleReservation>(entity =>
        {
            entity.HasKey(x => x.Id);
            entity.Property(x => x.BookingReference).IsRequired().HasMaxLength(100);
            entity.HasOne(x => x.Vehicle)
                .WithMany(x => x.Reservations)
                .HasForeignKey(x => x.VehicleId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }
}
