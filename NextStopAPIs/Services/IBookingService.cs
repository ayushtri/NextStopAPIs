using NextStopAPIs.DTOs;

namespace NextStopAPIs.Services
{
    public interface IBookingService
    {
        Task<IEnumerable<BusSearchResultDTO>> SearchBus(SearchBusDTO searchBusDto);
        Task<BookingDTO> BookTicket(BookTicketDTO bookTicketDto);
        Task<bool> CancelBooking(int bookingId); 
        Task<BookingDTO> GetBookingById(int bookingId);
        Task<IEnumerable<BookingDTO>> GetBookingsByUserId(int userId);
        Task<IEnumerable<BookingDTO>> GetBookingsByScheduleId(int scheduleId);
        Task<SeatLogDTO> GetSeatLogByBookingId(int bookingId);
    }
}
