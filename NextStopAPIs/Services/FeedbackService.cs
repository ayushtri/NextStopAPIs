using Microsoft.EntityFrameworkCore;
using NextStopAPIs.Data;
using NextStopAPIs.DTOs;
using NextStopAPIs.Models;

namespace NextStopAPIs.Services
{
    public class FeedbackService : IFeedbackService
    {
        private readonly NextStopDbContext _context;

        public FeedbackService(NextStopDbContext context)
        {
            _context = context;
        }
        public async Task<IEnumerable<FeedbackDTO>> GetAllFeedbacks()
        {
            return await _context.Feedbacks
                .Select(feedback => new FeedbackDTO
                {
                    FeedbackId = feedback.FeedbackId,
                    BookingId = feedback.BookingId,
                    Rating = feedback.Rating,
                    FeedbackText = feedback.FeedbackText
                })
                .ToListAsync();
        }

        public async Task<FeedbackDTO> GetFeedbackById(int feedbackId)
        {
            var feedback = await _context.Feedbacks.FindAsync(feedbackId);
            if (feedback == null) return null;

            return new FeedbackDTO
            {
                FeedbackId = feedback.FeedbackId,
                BookingId = feedback.BookingId,
                Rating = feedback.Rating,
                FeedbackText = feedback.FeedbackText
            };
        }

        public async Task<IEnumerable<FeedbackDTO>> GetFeedbacksByBookingId(int bookingId)
        {
            return await _context.Feedbacks
                .Where(feedback => feedback.BookingId == bookingId)
                .Select(feedback => new FeedbackDTO
                {
                    FeedbackId = feedback.FeedbackId,
                    BookingId = feedback.BookingId,
                    Rating = feedback.Rating,
                    FeedbackText = feedback.FeedbackText
                })
                .ToListAsync();
        }

        public async Task<FeedbackDTO> AddFeedback(AddFeedbackDTO feedbackDTO)
        {
            var booking = await _context.Bookings.FindAsync(feedbackDTO.BookingId);
            if (booking == null)
            {
                throw new ArgumentException("BookingId does not exist.");
            }

            var feedback = new Feedback
            {
                BookingId = feedbackDTO.BookingId,
                Rating = feedbackDTO.Rating,
                FeedbackText = feedbackDTO.FeedbackText
            };

            _context.Feedbacks.Add(feedback);
            await _context.SaveChangesAsync();

            var createdFeedback = new FeedbackDTO
            {
                FeedbackId = feedback.FeedbackId,
                BookingId = feedback.BookingId,
                Rating = feedback.Rating,
                FeedbackText = feedback.FeedbackText
            };

            return createdFeedback;
        }



        public async Task<Update_DeleteResponse> UpdateFeedback(int feedbackId, UpdateFeedbackDTO updateFeedbackDTO)
        {
            var feedback = await _context.Feedbacks.FindAsync(feedbackId);
            if (feedback == null)
            {
                return new Update_DeleteResponse { Success = false, Message = "Feedback not found." };
            }

            feedback.Rating = updateFeedbackDTO.Rating;
            feedback.FeedbackText = updateFeedbackDTO.FeedbackText;

            _context.Feedbacks.Update(feedback);
            await _context.SaveChangesAsync();

            return new Update_DeleteResponse { Success = true, Message = "Feedback updated successfully." };
        }


        public async Task DeleteFeedback(int feedbackId)
        {
            var feedback = await _context.Feedbacks.FindAsync(feedbackId);
            if (feedback == null) return;

            _context.Feedbacks.Remove(feedback);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<FeedbackResponseDTO>> GetFeedbacksByBusId(int busId)
        {
            var feedbacks = await _context.Feedbacks
                .Include(f => f.Booking)
                .ThenInclude(b => b.Schedule)
                .ThenInclude(s => s.Bus)
                .Where(f => f.Booking.Schedule.BusId == busId)
                .ToListAsync();

            return feedbacks.Select(f => new FeedbackResponseDTO
            {
                FeedbackId = f.FeedbackId,
                BookingId = f.BookingId,
                Rating = f.Rating,
                FeedbackText = f.FeedbackText
            });
        }


    }
}
