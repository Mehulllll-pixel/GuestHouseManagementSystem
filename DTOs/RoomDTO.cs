namespace GuestHouseAPI.DTOs
{
    public class RoomDTO
    {
        public string RoomNumber { get; set; } = string.Empty;

        public string RoomType { get; set; } = string.Empty;

        public string Status { get; set; } = "Available";

        public int Capacity { get; set; }
    }
}