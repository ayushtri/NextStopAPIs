﻿using System.ComponentModel.DataAnnotations;

namespace NextStopAPIs.Models
{
    public class Route
    {
        [Key]
        public int RouteId { get; set; }

        [Required]
        [StringLength(100)]
        public string Origin { get; set; }

        [Required]
        [StringLength(100)]
        public string Destination { get; set; }

        [Range(0, double.MaxValue)]
        public decimal Distance { get; set; }

        [StringLength(50)]
        public string EstimatedTime { get; set; }

        // Navigation Property
        public virtual ICollection<Schedule> Schedules { get; set; }
    }
}
