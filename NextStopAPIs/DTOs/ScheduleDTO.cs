namespace NextStopAPIs.DTOs
{
    public class ScheduleDTO
    {
        public int ScheduleId { get; set; }
        public int BusId { get; set; }
        public string BusName { get; set; }
        public int RouteId { get; set; }
        public string Origin { get; set; }
        public string Destination { get; set; }
        public DateTime DepartureTime { get; set; }
        public DateTime ArrivalTime { get; set; }
        public decimal Fare { get; set; }
        public DateTime Date { get; set; }
        public List<ScheduleSeatDTO> Seats { get; set; }
    }
}
