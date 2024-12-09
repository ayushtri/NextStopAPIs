using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace NextStopAPIs.Models
{
    public class ScheduleSeats
    {
        [Key]
        public int ScheduleSeatId { get; set; }

        [ForeignKey("Schedule")]
        public int ScheduleId { get; set; }
        public virtual Schedule Schedule { get; set; }

        [ForeignKey("Seat")]
        public int SeatId { get; set; }
        public virtual Seat Seat { get; set; }

        [Required]
        [StringLength(10)]
        public string SeatNumber { get; set; }

        [ForeignKey("Booking")]
        public int BookingId { get; set; }
        public virtual Booking Booking { get; set; }
    }
}
