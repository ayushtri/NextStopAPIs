namespace NextStopAPIs.DTOs
{
    public class FeedbackDTO
    {
        public int FeedbackId { get; set; } 
        public int BookingId { get; set; } 
        public int Rating { get; set; } 
        public string FeedbackText { get; set; }
    }
}
