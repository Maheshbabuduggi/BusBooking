using Microsoft.EntityFrameworkCore;
using BookingApi.Data.Entities;

namespace BookingApi.Data;

public class BookingDbContext : DbContext
{
    public BookingDbContext(DbContextOptions<BookingDbContext> options) : base(options) { }

    public DbSet<Booking> Bookings => Set<Booking>();
    public DbSet<BookingSeat> BookingSeats => Set<BookingSeat>();
    public DbSet<Payment> Payments => Set<Payment>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Booking>(e =>
        {
            e.ToTable("Bookings");
            e.HasKey(x => x.Id);
            e.Property(x => x.CustomerName).IsRequired().HasMaxLength(200);
            e.Property(x => x.CustomerEmail).IsRequired().HasMaxLength(200);
            e.Property(x => x.CustomerPhone).IsRequired().HasMaxLength(20);
            e.Property(x => x.TotalFare).HasColumnType("decimal(10,2)");
            e.Property(x => x.Status).IsRequired().HasMaxLength(50);
        });

        modelBuilder.Entity<BookingSeat>(e =>
        {
            e.ToTable("BookingSeats");
            e.HasKey(x => x.Id);
            e.Property(x => x.SeatNumber).IsRequired().HasMaxLength(10);
            e.HasOne(x => x.Booking).WithMany().HasForeignKey(x => x.BookingId);
        });

        modelBuilder.Entity<Payment>(e =>
        {
            e.ToTable("Payments");
            e.HasKey(x => x.Id);
            e.Property(x => x.Amount).HasColumnType("decimal(10,2)");
            e.Property(x => x.Status).IsRequired().HasMaxLength(50);
            e.Property(x => x.TransactionRef).HasMaxLength(100);
            e.HasOne(x => x.Booking).WithMany().HasForeignKey(x => x.BookingId);
        });

        base.OnModelCreating(modelBuilder);
    }
}
