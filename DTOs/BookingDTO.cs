namespace GuestHouseAPI.DTOs
{
    public class BookingDTO
    {
        public int UserId { get; set; }

        public string RoomType { get; set; } = string.Empty;

        public DateTimeOffset CheckInDate { get; set; }

        public DateTimeOffset CheckOutDate { get; set; }
    }
}