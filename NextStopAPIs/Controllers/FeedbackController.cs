using log4net;
using Microsoft.AspNetCore.Mvc;
using NextStopAPIs.DTOs;
using NextStopAPIs.Models;
using NextStopAPIs.Services;

namespace NextStopAPIs.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FeedbackController : ControllerBase    
    {
        private readonly IFeedbackService _feedbackService;
        private readonly ILog _logger;


        public FeedbackController(IFeedbackService feedbackService, ILog logger)
        {
            _feedbackService = feedbackService;
            _logger = logger;
        }

        [HttpGet("GetAllFeedbacks")]
        public async Task<ActionResult<IEnumerable<FeedbackResponseDTO>>> GetAllFeedbacks()
        {
            try
            {
                var feedbacks = await _feedbackService.GetAllFeedbacks();
                var response = feedbacks.Select(ToResponseDto).ToList();
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.Error("An error occurred while fetching feedbacks", ex);
                return StatusCode(500, new { Message = "An error occurred while fetching feedbacks.", Details = ex.Message });
            }
        }

        [HttpGet("GetFeedbackById/{id}")]
        public async Task<ActionResult<FeedbackResponseDTO>> GetFeedbackById(int id)
        {
            try
            {
                var feedback = await _feedbackService.GetFeedbackById(id);
                if (feedback == null)
                {
                    return NotFound(new { Message = "Feedback not found." });
                }

                return Ok(ToResponseDto(feedback));
            }
            catch (Exception ex)
            {
                _logger.Error("An error occurred while fetching feedback", ex);
                return StatusCode(500, new { Message = "An error occurred while fetching the feedback.", Details = ex.Message });
            }
        }

        [HttpGet("GetFeedbacksByBookingId/{bookingId}")]
        public async Task<ActionResult<IEnumerable<FeedbackResponseDTO>>> GetFeedbacksByBookingId(int bookingId)
        {
            try
            {
                var feedbacks = await _feedbackService.GetFeedbacksByBookingId(bookingId);
                var response = feedbacks.Select(ToResponseDto).ToList();
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.Error("An error occurred while fetching feedback", ex);
                return StatusCode(500, new { Message = "An error occurred while fetching feedbacks by booking ID.", Details = ex.Message });
            }
        }

        [HttpPost("AddFeedback")]
        public async Task<ActionResult<FeedbackResponseDTO>> AddFeedback([FromBody] AddFeedbackDTO feedbackDTO)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var createdFeedbackDTO = await _feedbackService.AddFeedback(feedbackDTO);

                if (createdFeedbackDTO == null)
                {
                    return BadRequest(new { Message = "Feedback creation failed." });
                }

                var response = ToResponseDto(createdFeedbackDTO);
                return CreatedAtAction(nameof(GetFeedbackById), new { id = createdFeedbackDTO.FeedbackId }, response);
            }
            catch (Exception ex)
            {
                _logger.Error("An error occurred while creating feedback", ex);
                return StatusCode(500, new { Message = "An error occurred while adding feedback.", Details = ex.Message });
            }
        }

        [HttpPut("UpdateFeedback/{id}")]
        public async Task<ActionResult<Update_DeleteResponse>> UpdateFeedback(int id, [FromBody] UpdateFeedbackDTO updateFeedbackDTO)
        {
            try
            {
                var existingFeedback = await _feedbackService.GetFeedbackById(id);
                if (existingFeedback == null)
                {
                    return NotFound(new Update_DeleteResponse { Success = false, Message = "Feedback not found." });
                }

                var updateResponse = await _feedbackService.UpdateFeedback(id, updateFeedbackDTO);

                if (!updateResponse.Success)
                {
                    return BadRequest(updateResponse);
                }

                return Ok(updateResponse); 
            }
            catch (Exception ex)
            {
                _logger.Error("An error occurred while updating feedbacks", ex);
                return StatusCode(500, new Update_DeleteResponse { Success = false, Message = "An error occurred while updating feedback.", Details = ex.Message });
            }
        }


        [HttpDelete("DeleteFeedback/{id}")]
        public async Task<ActionResult<Update_DeleteResponse>> DeleteFeedback(int id)
        {
            try
            {
                var existingFeedback = await _feedbackService.GetFeedbackById(id);
                if (existingFeedback == null)
                {
                    return NotFound(new { Message = "Feedback not found." });
                }

                await _feedbackService.DeleteFeedback(id);
                return new Update_DeleteResponse { Success = true, Message = "Feedback deleted successfully." };

            }
            catch (Exception ex)
            {
                _logger.Error("An error occurred while deleting feedbacks", ex);
                return StatusCode(500, new { Message = "An error occurred while deleting feedback.", Details = ex.Message });
            }
        }

        [HttpGet("GetFeedbacksByBusId/{busId}")]
        public async Task<ActionResult<IEnumerable<FeedbackResponseDTO>>> GetFeedbacksByBusId(int busId)
        {
            try
            {
                var feedbacks = await _feedbackService.GetFeedbacksByBusId(busId);
                if (!feedbacks.Any())
                {
                    return NotFound(new { Message = "No feedback found for the specified Bus ID." });
                }
                return Ok(feedbacks);
            }
            catch (Exception ex)
            {
                _logger.Error("An error occurred while fetching feedbacks", ex);
                return StatusCode(500, new { Message = "An error occurred while fetching feedbacks by Bus ID.", Details = ex.Message });
            }
        }

        private FeedbackResponseDTO ToResponseDto(Feedback feedback)
        {
            return new FeedbackResponseDTO
            {
                FeedbackId = feedback.FeedbackId,
                BookingId = feedback.BookingId,
                Rating = feedback.Rating,
                FeedbackText = feedback.FeedbackText
            };
        }

        private FeedbackResponseDTO ToResponseDto(FeedbackDTO feedbackDTO)
        {
            return new FeedbackResponseDTO
            {
                FeedbackId = feedbackDTO.FeedbackId,
                BookingId = feedbackDTO.BookingId,
                Rating = feedbackDTO.Rating,
                FeedbackText = feedbackDTO.FeedbackText
            };
        }
    }
}
