using System.Text.Json.Serialization;

namespace GuestHouseAPI.Models
{
    public class User
    {
        public int UserId { get; set; }

        public string FullName { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        [JsonIgnore]
        public string PasswordHash { get; set; } = string.Empty;

        public string? EmployeeId { get; set; }

        public string PhoneNumber { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public int RoleId { get; set; }

        public Role? Role { get; set; }

        [JsonIgnore]
        public ICollection<Booking> Bookings { get; set; } = new List<Booking>();

        [JsonIgnore]
        public ICollection<Maintenance> Maintenances { get; set; } = new List<Maintenance>();

        [JsonIgnore]
        public ICollection<Notification> Notifications { get; set; } = new List<Notification>();
    }
}