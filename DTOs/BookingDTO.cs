namespace GuestHouseAPI.DTOs
{
    public class BookingDTO
    {
        public int UserId { get; set; }

        public string RoomType { get; set; } = string.Empty;

        public DateTime CheckInDate { get; set; }

        public DateTime CheckOutDate { get; set; }
    }
}