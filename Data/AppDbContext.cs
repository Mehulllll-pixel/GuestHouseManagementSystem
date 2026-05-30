using GuestHouseAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace GuestHouseAPI.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<Role> Roles { get; set; }

        public DbSet<User> Users { get; set; }

        public DbSet<Room> Rooms { get; set; }

        public DbSet<Booking> Bookings { get; set; }

        public DbSet<Maintenance> Maintenances { get; set; }

        public DbSet<Notification> Notifications { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Role -> Users
            modelBuilder.Entity<User>()
                .HasOne(u => u.Role)
                .WithMany(r => r.Users)
                .HasForeignKey(u => u.RoleId);

            // User -> Bookings
            modelBuilder.Entity<Booking>()
                .HasOne(b => b.User)
                .WithMany(u => u.Bookings)
                .HasForeignKey(b => b.UserId);

            // Room -> Bookings
            modelBuilder.Entity<Booking>()
                .HasOne(b => b.Room)
                .WithMany(r => r.Bookings)
                .HasForeignKey(b => b.RoomId);

            // Room -> Maintenance
            modelBuilder.Entity<Maintenance>()
                .HasOne(m => m.Room)
                .WithMany(r => r.Maintenances)
                .HasForeignKey(m => m.RoomId);

            // User -> Maintenance
            modelBuilder.Entity<Maintenance>()
                .HasOne(m => m.ReportedByUser)
                .WithMany(u => u.Maintenances)
                .HasForeignKey(m => m.ReportedByUserId)
                .OnDelete(DeleteBehavior.Restrict);

            // User -> Notifications
            modelBuilder.Entity<Notification>()
                .HasOne(n => n.User)
                .WithMany(u => u.Notifications)
                .HasForeignKey(n => n.UserId);

            // Seed Roles
            modelBuilder.Entity<Role>().HasData(
                new Role { RoleId = 1, RoleName = "Admin" },
                new Role { RoleId = 2, RoleName = "Management" },
                new Role { RoleId = 3, RoleName = "Security" },
                new Role { RoleId = 4, RoleName = "Staff" },
                new Role { RoleId = 5, RoleName = "Guest" }
            );
        }
    }
}