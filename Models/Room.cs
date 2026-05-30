namespace GuestHouseAPI.Models
{
    public class Room
    {
        public int RoomId { get; set; }

        public string RoomNumber { get; set; } = string.Empty;

        public string RoomType { get; set; } = string.Empty;

        public string Status { get; set; } = "Available";

        public int Capacity { get; set; }

        public ICollection<Booking> Bookings { get; set; } = new List<Booking>();

        public ICollection<Maintenance> Maintenances { get; set; } = new List<Maintenance>();
    }
}