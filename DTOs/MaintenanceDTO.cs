namespace GuestHouseAPI.DTOs
{
    public class MaintenanceDTO
    {
        public int RoomId { get; set; }

        public int ReportedByUserId { get; set; }

        public string Reason { get; set; } = string.Empty;
    }
}