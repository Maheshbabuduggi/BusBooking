using Microsoft.EntityFrameworkCore;
using CatalogApi.Data.Entities;

namespace CatalogApi.Data;

public class CatalogDbContext : DbContext
{
    public CatalogDbContext(DbContextOptions<CatalogDbContext> options) : base(options) { }

    public DbSet<Bus> Buses => Set<Bus>();
    public DbSet<CatalogApi.Data.Entities.Route> Routes => Set<CatalogApi.Data.Entities.Route>();
    public DbSet<Trip> Trips => Set<Trip>();
    public DbSet<Seat> Seats => Set<Seat>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Bus>(e =>
        {
            e.ToTable("Buses");
            e.HasKey(x => x.Id);
            e.Property(x => x.RegistrationNumber).IsRequired().HasMaxLength(50);
            e.Property(x => x.OperatorName).IsRequired().HasMaxLength(200);
            e.Property(x => x.BusType).IsRequired().HasMaxLength(50);
        });

        modelBuilder.Entity<CatalogApi.Data.Entities.Route>(e =>
        {
            e.ToTable("Routes");
            e.HasKey(x => x.Id);
            e.Property(x => x.Origin).IsRequired().HasMaxLength(100);
            e.Property(x => x.Destination).IsRequired().HasMaxLength(100);
            e.Property(x => x.DistanceKm).HasColumnType("decimal(10,2)");
        });

        modelBuilder.Entity<Trip>(e =>
        {
            e.ToTable("Trips");
            e.HasKey(x => x.Id);
            e.Property(x => x.FarePerSeat).HasColumnType("decimal(10,2)");
            e.Property(x => x.Status).IsRequired().HasMaxLength(50);
            e.HasOne(x => x.Route).WithMany().HasForeignKey(x => x.RouteId);
            e.HasOne(x => x.Bus).WithMany().HasForeignKey(x => x.BusId);
        });

        modelBuilder.Entity<Seat>(e =>
        {
            e.ToTable("Seats");
            e.HasKey(x => x.Id);
            e.Property(x => x.SeatNumber).IsRequired().HasMaxLength(10);
            e.Property(x => x.Status).IsRequired().HasMaxLength(20);
            e.HasOne(x => x.Trip).WithMany(t => t.Seats).HasForeignKey(x => x.TripId);
        });

        base.OnModelCreating(modelBuilder);
    }
}
