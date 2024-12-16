using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NextStopAPIs.Models
{
    public class Feedback
    {
        [Key]
        public int FeedbackId { get; set; } 

        [Required]
        [Range(1, 5, ErrorMessage = "Rating must be between 1 and 5.")]
        public int Rating { get; set; } 

        [StringLength(500)]
        public string FeedbackText { get; set; } 

        [Required]
        [ForeignKey("Booking")]
        public int BookingId { get; set; } 
        public virtual Booking Booking { get; set; } 
    }
}
