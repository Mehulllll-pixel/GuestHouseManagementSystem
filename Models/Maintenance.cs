namespace GuestHouseAPI.Models
{
    public class Maintenance
    {
        public int MaintenanceId { get; set; }

        public int RoomId { get; set; }

        public Room? Room { get; set; }

        public int ReportedByUserId { get; set; }

        public User? ReportedByUser { get; set; }

        public string Reason { get; set; } = string.Empty;

        public string Status { get; set; } = "Open";

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}