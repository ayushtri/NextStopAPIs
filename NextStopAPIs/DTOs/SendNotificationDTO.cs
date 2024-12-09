using NextStopAPIs.Enums;
using System.ComponentModel.DataAnnotations;

namespace NextStopAPIs.DTOs
{
    public class SendNotificationDTO
    {
        [Required]
        public int UserId { get; set; }

        [Required]
        [StringLength(255)]
        public string Message { get; set; }

        [Required]
        public NotificationTypeEnum NotificationType { get; set; }
    }
}
