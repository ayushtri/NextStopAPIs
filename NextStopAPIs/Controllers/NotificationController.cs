using log4net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NextStopAPIs.DTOs;
using NextStopAPIs.Services;
using System;
using System.Threading.Tasks;

namespace NextStopAPIs.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NotificationController : ControllerBase
    {
        private readonly INotificationService _notificationService;
        private readonly ILog _logger;

        public NotificationController(INotificationService notificationService, ILog logger)
        {
            _notificationService = notificationService;
            _logger = logger;
        }

        [HttpPost("SendNotification")]
        [Authorize(Roles = "passenger,operator,admin")]
        public async Task<IActionResult> SendNotification([FromBody] SendNotificationDTO sendNotificationDto)
        {
            try
            {
                var result = await _notificationService.SendNotification(sendNotificationDto);

                if (!result) 
                {
                    return BadRequest(new NotifResDTO { Message = "Failed to send notification." });
                }

                var response = new NotifResDTO
                {
                    Success = result,
                    Message = result ? "Notification sent successfully." : "Failed to send notification."
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.Error("Error sending notification", ex);
                return StatusCode(500, new NotifResDTO { Message = "An error occurred while sending the notification." });
            }
        }

        [HttpGet("ViewNotifications/{userId}")]
        [Authorize(Roles = "passenger,operator,admin")]
        public async Task<IActionResult> ViewNotifications(int userId)
        {
            try
            {
                var notifications = await _notificationService.ViewNotifications(userId);
                if (notifications.Any())
                {
                    return Ok(notifications);
                }

                return NotFound("No notifications found for this user.");
            }
            catch (Exception ex)
            {
                _logger.Error($"Error fetching notifications for user ID {userId}", ex);
                return StatusCode(500, "An error occurred while fetching notifications.");
            }
        }


        [HttpPost("MarkNotificationAsRead/{notificationId}")]
        [Authorize(Roles = "passenger,operator,admin")]
        public async Task<IActionResult> MarkNotificationAsRead(int notificationId)
        {
            try
            {
                var result = await _notificationService.MarkNotificationAsRead(notificationId);

                if (result == null)
                {
                    return NotFound(new { Message = "Notification not found." });
                }

                return Ok(result); // Return NotificationResponseDTO directly
            }
            catch (Exception ex)
            {
                _logger.Error($"Error marking notification {notificationId} as read", ex);
                return StatusCode(500, new { Message = "An error occurred while marking the notification as read." });
            }
        }


    }
}
