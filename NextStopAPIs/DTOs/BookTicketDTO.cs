namespace NextStopAPIs.DTOs
{
    public class BookTicketDTO
    {
        public int UserId { get; set; }
        public int ScheduleId { get; set; }
        public List<string> SeatNumbers { get; set; }  
    }
}
