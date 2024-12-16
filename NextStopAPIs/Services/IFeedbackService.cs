using NextStopAPIs.DTOs;

namespace NextStopAPIs.Services
{
    public interface IFeedbackService
    {
        Task<IEnumerable<FeedbackDTO>> GetAllFeedbacks();
        Task<FeedbackDTO> GetFeedbackById(int feedbackId); 
        Task<IEnumerable<FeedbackDTO>> GetFeedbacksByBookingId(int bookingId);
        Task<FeedbackDTO> AddFeedback(AddFeedbackDTO feedbackDTO);
        Task<Update_DeleteResponse> UpdateFeedback(int feedbackId, UpdateFeedbackDTO updateFeedbackDTO);
        Task DeleteFeedback(int feedbackId);
        Task<IEnumerable<FeedbackResponseDTO>> GetFeedbacksByBusId(int busId);
    }
}
