using Microsoft.EntityFrameworkCore;
using NextStopAPIs.Models;

namespace NextStopAPIs.Data
{
    public class NextStopDbContext : DbContext
    {
        public NextStopDbContext() { }
        public NextStopDbContext(DbContextOptions<NextStopDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configure ScheduleSeats to Booking as optional
            modelBuilder.Entity<ScheduleSeats>()
                .HasOne(s => s.Booking)
                .WithMany(b => b.ScheduleSeats)
                .HasForeignKey(s => s.BookingId)
                .OnDelete(DeleteBehavior.Restrict);


            modelBuilder.Entity<ScheduleSeats>()
                .HasOne(ss => ss.Schedule)
                .WithMany(s => s.ScheduleSeats)
                .HasForeignKey(ss => ss.ScheduleId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ScheduleSeats>()
                 .HasOne(ss => ss.Seat)
                 .WithMany(seat => seat.ScheduleSeats)
                 .HasForeignKey(ss => ss.SeatId)
                 .OnDelete(DeleteBehavior.Restrict); // Prevent cascade delete for Seat


            // Unique constraint on BusNumber
            modelBuilder.Entity<Bus>()
                .HasIndex(b => b.BusNumber)
                .IsUnique();

            // Make Email property unique
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();

            // Set precision for decimal properties
            modelBuilder.Entity<Booking>()
                .Property(b => b.TotalFare)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<Payment>()
                .Property(p => p.Amount)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<Models.Route>()
                .Property(r => r.Distance)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<Schedule>()
                .Property(s => s.Fare)
                .HasColumnType("decimal(18,2)");

            // Configure foreign keys with restricted cascading behavior
            modelBuilder.Entity<Booking>()
                .HasOne(b => b.User)
                .WithMany(u => u.Bookings)
                .HasForeignKey(b => b.UserId)
                .OnDelete(DeleteBehavior.Restrict); // Avoid cascading on delete

            modelBuilder.Entity<Booking>()
                .HasOne(b => b.Schedule)
                .WithMany(s => s.Bookings)
                .HasForeignKey(b => b.ScheduleId)
                .OnDelete(DeleteBehavior.Cascade); // Keep cascading only where needed
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Bus> Buses { get; set; }
        public DbSet<Seat> Seats { get; set; }
        public DbSet<Booking> Bookings { get; set; }
        public DbSet<Schedule> Schedules { get; set; }
        public DbSet<Models.Route> Routes { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<AdminAction> AdminActions { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }
        public DbSet<SeatLog> SeatLogs { get; set; }
        public DbSet<ScheduleSeats> ScheduleSeats { get; set; }
        public DbSet<Feedback> Feedbacks { get; set; }
    }
}
