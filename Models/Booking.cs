namespace GuestHouseAPI.Models
{
    public class Booking
    {
        public int BookingId { get; set; }

        public int UserId { get; set; }

        public User? User { get; set; }

        public int RoomId { get; set; }

        public Room? Room { get; set; }

        public DateTimeOffset CheckInDate { get; set; }

        public DateTimeOffset CheckOutDate { get; set; }

        public string Status { get; set; } = "Pending";

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}