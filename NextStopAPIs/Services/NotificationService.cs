using NextStopAPIs.DTOs;
using NextStopAPIs.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NextStopAPIs.Data;

namespace NextStopAPIs.Services
{
    public class NotificationService : INotificationService
    {
        private readonly NextStopDbContext _context;

        public NotificationService(NextStopDbContext context)
        {
            _context = context;
        }

        public async Task<bool> SendNotification(SendNotificationDTO sendNotificationDto)
        {
            try
            {
                var notification = new Notification
                {
                    UserId = sendNotificationDto.UserId,
                    Message = sendNotificationDto.Message,
                    NotificationType = sendNotificationDto.NotificationType,
                    SentDate = DateTime.Now
                };

                _context.Notifications.Add(notification);
                await _context.SaveChangesAsync();



                return true;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error sending notification: {ex.Message}");
            }
        }

        public async Task<IEnumerable<NotificationDTO>> ViewNotifications(int userId)
        {
            try
            {
                var notifications = await _context.Notifications
                    .Where(n => n.UserId == userId)
                    .OrderByDescending(n => n.SentDate) // Order by most recent
                    .ToListAsync();

                return notifications.Select(n => new NotificationDTO
                {
                    NotificationId = n.NotificationId,
                    UserId = n.UserId,
                    Message = n.Message,
                    SentDate = n.SentDate,
                    NotificationType = n.NotificationType,
                    IsRead = n.IsRead
                });
            }
            catch (Exception ex)
            {
                throw new Exception($"Error fetching notifications: {ex.Message}");
            }
        }

        public async Task<NotificationResponseDTO> MarkNotificationAsRead(int notificationId)
        {
            try
            {
                var notification = await _context.Notifications
                    .FirstOrDefaultAsync(n => n.NotificationId == notificationId);

                if (notification == null)
                {
                    return null; // Notification not found
                }

                // Mark as read and save changes
                notification.IsRead = true;
                await _context.SaveChangesAsync();

                // Map the updated notification to NotificationResponseDTO
                var notificationResponse = new NotificationResponseDTO
                {
                    NotificationId = notification.NotificationId,
                    Message = notification.Message,
                    IsRead = notification.IsRead,
                    Timestamp = notification.SentDate
                };

                return notificationResponse;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error marking notification as read: {ex.Message}");
            }
        }


    }
}
