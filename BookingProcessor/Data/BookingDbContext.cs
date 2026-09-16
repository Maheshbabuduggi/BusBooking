using Microsoft.EntityFrameworkCore;
using BookingProcessor.Data.Entities;

namespace BookingProcessor.Data;

public class BookingDbContext : DbContext
{
    public BookingDbContext(DbContextOptions<BookingDbContext> options) : base(options) { }

    public DbSet<Booking> Bookings => Set<Booking>();
    public DbSet<BookingSeat> BookingSeats => Set<BookingSeat>();
    public DbSet<Payment> Payments => Set<Payment>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Booking>().ToTable("Bookings").HasKey(x => x.Id);
        modelBuilder.Entity<BookingSeat>().ToTable("BookingSeats").HasKey(x => x.Id);
        modelBuilder.Entity<Payment>().ToTable("Payments").HasKey(x => x.Id);
        base.OnModelCreating(modelBuilder);
    }
}
