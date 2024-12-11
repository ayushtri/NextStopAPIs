using NextStopAPIs.DTOs;

namespace NextStopAPIs.Services
{
    public interface INotificationService
    {
        Task<bool> SendNotification(SendNotificationDTO sendNotificationDto);
        Task<IEnumerable<NotificationDTO>> ViewNotifications(int userId);
        Task<NotificationResponseDTO> MarkNotificationAsRead(int notificationId);
    }
}
