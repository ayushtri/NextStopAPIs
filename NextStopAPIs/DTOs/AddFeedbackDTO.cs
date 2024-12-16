namespace NextStopAPIs.DTOs
{
    public class AddFeedbackDTO
    {
        public int BookingId { get; set; }
        public int Rating { get; set; }
        public string FeedbackText { get; set; }
    }

}
