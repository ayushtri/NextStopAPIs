using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace NextStopAPIs.Models
{
    public class Seat
    {
        [Key]
        public int SeatId { get; set; }

        [ForeignKey("Bus")]
        public int BusId { get; set; }
        public virtual Bus Bus { get; set; }

        [Required]
        [StringLength(10)]
        public string SeatNumber { get; set; } // Optional: Seat number (e.g., A1, B2)

        // Navigation property to ScheduleSeats
        public virtual ICollection<ScheduleSeats> ScheduleSeats { get; set; }
    }
}
