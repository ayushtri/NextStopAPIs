using NextStopAPIs.DTOs;
using NextStopAPIs.Models;
using NextStopAPIs.Data;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using log4net;

namespace NextStopAPIs.Services
{
    public class BookingService : IBookingService
    {
        private readonly NextStopDbContext _context;
        private readonly ILog _logger;

        public BookingService(NextStopDbContext context, ILog logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IEnumerable<BusSearchResultDTO>> SearchBus(SearchBusDTO searchBusDto)
        {
            var results = from schedule in _context.Schedules
                          join route in _context.Routes on schedule.RouteId equals route.RouteId
                          join bus in _context.Buses on schedule.BusId equals bus.BusId
                          where route.Origin == searchBusDto.Origin
                          && route.Destination == searchBusDto.Destination
                          // Ensure the travel date matches the schedule's date (ignoring time)
                          && schedule.DepartureTime.Date == searchBusDto.TravelDate.Date
                          select new BusSearchResultDTO
                          {
                              ScheduleId = schedule.ScheduleId,
                              BusId = bus.BusId,
                              RouteId = route.RouteId,
                              DepartureTime = schedule.DepartureTime,
                              ArrivalTime = schedule.ArrivalTime,
                              Fare = schedule.Fare,
                              BusName = bus.BusName,
                              Origin = route.Origin,
                              Destination = route.Destination,
                              // Calculate available seats based on total seats and booked seats
                              AvailableSeats = _context.Seats.Count(s => s.BusId == bus.BusId) -
                                               _context.ScheduleSeats.Count(ss => ss.ScheduleId == schedule.ScheduleId && ss.BookingId != null)
                          };

            return await results.ToListAsync();
        }



        public async Task<BookingDTO> BookTicket(BookTicketDTO bookTicketDto)
        {
            try
            {
                _logger.Info($"Booking request started for UserId: {bookTicketDto.UserId}, ScheduleId: {bookTicketDto.ScheduleId}, SeatNumbers: {string.Join(", ", bookTicketDto.SeatNumbers)}");

                // Get the schedule details along with its associated ScheduleSeats
                var schedule = await _context.Schedules
                                              .Include(s => s.ScheduleSeats)
                                              .FirstOrDefaultAsync(s => s.ScheduleId == bookTicketDto.ScheduleId);

                if (schedule == null)
                {
                    _logger.Error($"Schedule not found for ScheduleId: {bookTicketDto.ScheduleId}");
                    throw new Exception("Schedule not found");
                }

                // Get the total number of seats for the bus associated with the schedule from the Seats table
                var totalSeatsOnBus = await _context.Seats
                                                     .Where(s => s.BusId == schedule.BusId)
                                                     .CountAsync();

                // Get the number of booked seats by checking ScheduleSeats for the current ScheduleId
                var bookedSeatsCount = await _context.ScheduleSeats
                                                      .Where(ss => ss.ScheduleId == schedule.ScheduleId && ss.BookingId != null)
                                                      .CountAsync();

                // Calculate available seats
                var availableSeatsCount = totalSeatsOnBus - bookedSeatsCount;

                _logger.Info($"Total Seats: {totalSeatsOnBus}, Booked Seats: {bookedSeatsCount}, Available Seats: {availableSeatsCount}");

                // Check if there are enough available seats for the booking
                if (bookTicketDto.SeatNumbers.Count > availableSeatsCount)
                {
                    _logger.Error($"Not enough available seats. Requested: {bookTicketDto.SeatNumbers.Count}, Available: {availableSeatsCount}");
                    throw new Exception($"Not enough available seats. Only {availableSeatsCount} seats are available.");
                }

                // Get the list of seats available for this schedule
                var bookedSeatNumbers = await _context.ScheduleSeats
                                                       .Where(ss => ss.ScheduleId == schedule.ScheduleId && ss.BookingId != null)
                                                       .Select(ss => ss.SeatNumber)
                                                       .ToListAsync();

                // Get the available seat numbers from the Seats table (those not booked)
                var availableSeats = await _context.Seats
                                                   .Where(s => s.BusId == schedule.BusId && !bookedSeatNumbers.Contains(s.SeatNumber))
                                                   .Select(s => s.SeatNumber)
                                                   .ToListAsync();

                // Ensure all requested seats are available
                if (bookTicketDto.SeatNumbers.Any(sn => !availableSeats.Contains(sn)))
                {
                    _logger.Error($"One or more requested seats are not available. Requested seats: {string.Join(", ", bookTicketDto.SeatNumbers)}");
                    throw new Exception("One or more seats are not available.");
                }

                // Create the booking
                var booking = new Booking
                {
                    UserId = bookTicketDto.UserId,
                    ScheduleId = bookTicketDto.ScheduleId,
                    TotalFare = schedule.Fare * bookTicketDto.SeatNumbers.Count,
                    Status = "confirmed",
                    BookingDate = DateTime.Now
                };

                // Log booking creation
                _logger.Info($"Creating booking for UserId: {bookTicketDto.UserId}, ScheduleId: {bookTicketDto.ScheduleId}, TotalFare: {booking.TotalFare}");

                // Add the new booking to the database
                _context.Bookings.Add(booking);
                await _context.SaveChangesAsync();

                // Create ScheduleSeats records for the selected seats and associate with the booking
                foreach (var seatNumber in bookTicketDto.SeatNumbers)
                {
                    var seat = await _context.Seats
                                              .FirstOrDefaultAsync(s => s.BusId == schedule.BusId && s.SeatNumber == seatNumber);

                    if (seat == null)
                    {
                        _logger.Error($"Seat {seatNumber} not found on this bus (BusId: {schedule.BusId})");
                        throw new Exception($"Seat {seatNumber} not found on this bus.");
                    }

                    // Create the ScheduleSeat record for each selected seat
                    var scheduleSeat = new ScheduleSeats
                    {
                        ScheduleId = bookTicketDto.ScheduleId,
                        SeatId = seat.SeatId,
                        SeatNumber = seatNumber,
                        BookingId = booking.BookingId // Associate with the newly created booking
                    };

                    // Add the ScheduleSeat to the context
                    _context.ScheduleSeats.Add(scheduleSeat);
                }

                // Save the changes to the ScheduleSeats
                await _context.SaveChangesAsync();

                

                // Log the successful booking creation
                _logger.Info($"Booking successful for UserId: {bookTicketDto.UserId}, BookingId: {booking.BookingId}");

                var seatLog = new SeatLog
                {
                    BookingId = booking.BookingId,
                    BusId = schedule.BusId,
                    Seats = string.Join(",", bookTicketDto.SeatNumbers),
                    DateBooked = DateTime.Now
                };
                _context.SeatLogs.Add(seatLog);
                await _context.SaveChangesAsync();

                // Return a BookingDTO with the booking and reserved seat information
                return new BookingDTO
                {
                    BookingId = booking.BookingId,
                    UserId = booking.UserId,
                    ScheduleId = booking.ScheduleId,
                    ReservedSeats = bookTicketDto.SeatNumbers,
                    TotalFare = booking.TotalFare,
                    Status = booking.Status,
                    BookingDate = booking.BookingDate
                };
            }
            catch (Exception ex)
            {
                _logger.Error("Error occurred while booking ticket", ex);
                throw new Exception("An error occurred while booking the ticket", ex);
            }
        }




        public async Task<bool> CancelBooking(int bookingId)
        {
            var booking = await _context.Bookings
                                         .Include(b => b.ScheduleSeats)
                                         .FirstOrDefaultAsync(b => b.BookingId == bookingId);

            if (booking == null)
            {
                return false;  // Booking not found
            }

            // Mark the booking as canceled
            booking.Status = "cancelled";

            // Remove ScheduleSeat records associated with the booking
            var scheduleSeatsToRemove = booking.ScheduleSeats.ToList();
            _context.ScheduleSeats.RemoveRange(scheduleSeatsToRemove); // Delete the ScheduleSeats records

            await _context.SaveChangesAsync();
            return true;
        }


        public async Task<BookingDTO> GetBookingById(int bookingId)
        {
            var booking = await _context.Bookings
                                         .Include(b => b.ScheduleSeats)
                                         .ThenInclude(ss => ss.Seat)
                                         .FirstOrDefaultAsync(b => b.BookingId == bookingId);

            if (booking == null)
            {
                return null;
            }

            return new BookingDTO
            {
                BookingId = booking.BookingId,
                UserId = booking.UserId,
                ScheduleId = booking.ScheduleId,
                ReservedSeats = booking.ScheduleSeats.Select(ss => ss.SeatNumber).ToList(),
                TotalFare = booking.TotalFare,
                Status = booking.Status,
                BookingDate = booking.BookingDate
            };
        }

        public async Task<IEnumerable<BookingDTO>> GetBookingsByUserId(int userId)
        {
            var bookings = await _context.Bookings
                                          .Where(b => b.UserId == userId)
                                          .Include(b => b.ScheduleSeats)
                                          .ToListAsync();

            return bookings.Select(b => new BookingDTO
            {
                BookingId = b.BookingId,
                UserId = b.UserId,
                ScheduleId = b.ScheduleId,
                ReservedSeats = b.ScheduleSeats.Select(ss => ss.SeatNumber).ToList(),
                TotalFare = b.TotalFare,
                Status = b.Status,
                BookingDate = b.BookingDate
            }).ToList();
        }

        public async Task<IEnumerable<BookingDTO>> GetBookingsByScheduleId(int scheduleId)
        {
            var bookings = await _context.Bookings
                                          .Where(b => b.ScheduleId == scheduleId)
                                          .Include(b => b.ScheduleSeats)
                                          .ToListAsync();

            return bookings.Select(b => new BookingDTO
            {
                BookingId = b.BookingId,
                UserId = b.UserId,
                ScheduleId = b.ScheduleId,
                ReservedSeats = b.ScheduleSeats.Select(ss => ss.SeatNumber).ToList(),
                TotalFare = b.TotalFare,
                Status = b.Status,
                BookingDate = b.BookingDate
            }).ToList();
        }

        // Get SeatLog by BookingId
        public async Task<SeatLogDTO> GetSeatLogByBookingId(int bookingId)
        {
            try
            {
                // Retrieve the seat log based on BookingId
                var seatLog = await _context.SeatLogs
                    .Where(sl => sl.BookingId == bookingId)
                    .FirstOrDefaultAsync();

                if (seatLog == null)
                {
                    return null;  // Return null if no seat log is found for the given bookingId
                }

                // Map SeatLog entity to SeatLogDTO for response
                return new SeatLogDTO
                {
                    SeatLogId = seatLog.SeatLogId,
                    BookingId = seatLog.BookingId,
                    BusId = seatLog.BusId,
                    Seats = seatLog.Seats,  // Comma-separated list of booked seats
                    DateBooked = seatLog.DateBooked
                };
            }
            catch (Exception ex)
            {
                throw new Exception($"Error fetching seat log for BookingId {bookingId}: {ex.Message}");
            }
        }
    }
}
