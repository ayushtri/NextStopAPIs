using log4net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NextStopAPIs.DTOs;
using NextStopAPIs.Services;

namespace NextStopAPIs.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookingController : ControllerBase
    {
        private readonly IBookingService _bookingService;
        private readonly ILog _logger;

        public BookingController(IBookingService bookingService, ILog logger)
        {
            _bookingService = bookingService;
            _logger = logger;
        }

        [HttpPost("SearchBus")]
        public async Task<IActionResult> SearchBus([FromBody] SearchBusDTO searchBusDto)
        {
            try
            {
                var buses = await _bookingService.SearchBus(searchBusDto);
                return Ok(buses);
            }
            catch (Exception ex)
            {
                _logger.Error("Error searching buses", ex);
                return StatusCode(500, "An error occurred while searching for buses.");
            }
        }

        // Book a ticket
        [HttpPost("BookTicket")]
        [Authorize(Roles = "passenger,operator,admin")]
        public async Task<IActionResult> BookTicket([FromBody] BookTicketDTO bookTicketDto)
        {
            try
            {
                var booking = await _bookingService.BookTicket(bookTicketDto);

                if (booking == null)
                {
                    return BadRequest("Unable to book ticket. Please check the provided details.");
                }

                return CreatedAtAction(nameof(ViewBookingsByUserId), new { userId = bookTicketDto.UserId }, booking);
            }
            catch (Exception ex)
            {
                _logger.Error("Error booking ticket", ex);
                return StatusCode(500, "An error occurred while booking the ticket.");
            }
        }

        // Cancel a booking
        [HttpPost("CancelBooking")]
        [Authorize(Roles = "passenger,operator,admin")]
        public async Task<IActionResult> CancelBooking([FromBody] CancelBookingDTO cancelBookingDto)
        {
            try
            {
                var result = await _bookingService.CancelBooking(cancelBookingDto.BookingId);

                if (result)
                {
                    return Ok(new CancelBookingResponseDTO { Success = true, Message = "Booking cancelled successfully." });
                }

                return NotFound(new CancelBookingResponseDTO { Success = false, Message = "Booking not found or already cancelled." });
            }
            catch (Exception ex)
            {
                _logger.Error("Error canceling booking", ex);
                return StatusCode(500, new CancelBookingResponseDTO { Success = false, Message = "An error occurred while canceling the booking." });
            }
        }

        [HttpGet("ViewBookingByBookingId/{bookingId}")]
        [Authorize(Roles = "passenger,operator,admin")]
        public async Task<IActionResult> ViewBookingByBookingId(int bookingId)
        {
            try
            {
                var booking = await _bookingService.GetBookingById(bookingId);

                if (booking == null)
                {
                    return NotFound("Booking not found for this BookingId.");
                }

                return Ok(booking);
            }
            catch (Exception ex)
            {
                _logger.Error($"Error fetching booking for BookingId {bookingId}", ex);
                return StatusCode(500, "An error occurred while fetching the booking.");
            }
        }

        [HttpGet("ViewBookingsByUserId/{userId}")]
        [Authorize(Roles = "passenger,operator,admin")]
        public async Task<IActionResult> ViewBookingsByUserId(int userId)
        {
            try
            {
                var bookings = await _bookingService.GetBookingsByUserId(userId);

                if (!bookings.Any())
                {
                    return NotFound("No bookings found for this user.");
                }

                return Ok(bookings);
            }
            catch (Exception ex)
            {
                _logger.Error($"Error fetching bookings for user ID {userId}", ex);
                return StatusCode(500, "An error occurred while fetching bookings for the user.");
            }
        }

        [HttpGet("ViewBookingsByScheduleId/{scheduleId}")]
        [Authorize(Roles = "operator,admin")]
        public async Task<IActionResult> ViewBookingsByScheduleId(int scheduleId)
        {
            try
            {
                var bookings = await _bookingService.GetBookingsByScheduleId(scheduleId);

                if (!bookings.Any())
                {
                    return NotFound("No bookings found for this schedule.");
                }

                return Ok(bookings);
            }
            catch (Exception ex)
            {
                _logger.Error($"Error fetching bookings for schedule ID {scheduleId}", ex);
                return StatusCode(500, "An error occurred while fetching bookings for the schedule.");
            }
        }

        [HttpGet("GetSeatLogByBookingId/{bookingId}")]
        [Authorize(Roles = "passenger,operator,admin")]
        public async Task<IActionResult> GetSeatLogByBookingId(int bookingId)
        {
            try
            {
                var seatLog = await _bookingService.GetSeatLogByBookingId(bookingId);

                if (seatLog == null)
                {
                    return NotFound($"No seat log found for BookingId {bookingId}.");
                }

                return Ok(seatLog);
            }
            catch (Exception ex)
            {
                _logger.Error($"Error fetching seat log for BookingId {bookingId}", ex);
                return StatusCode(500, "An error occurred while fetching the seat log.");
            }
        }
    }
}
