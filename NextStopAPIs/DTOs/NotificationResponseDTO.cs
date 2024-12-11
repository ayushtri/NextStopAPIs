namespace NextStopAPIs.DTOs
{
    public class NotificationResponseDTO
    {
        public int NotificationId { get; set; }
        public string Message { get; set; }
        public bool IsRead { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
