using Microsoft.EntityFrameworkCore;
using NextStopAPIs.Data;
using NextStopAPIs.DTOs;
using NextStopAPIs.Enums;
using NextStopAPIs.Models;
using NextStopAPIs.Services;

namespace NextStopAPIs.Services
{
    public class BusService : IBusService
    {
        private readonly NextStopDbContext _context;

        public BusService(NextStopDbContext context)
        {
            _context = context;
        }

        public async Task<BusDTO> GetBusById(int busId)
        {
            try
            {
                var bus = await _context.Buses
                    .Include(b => b.Operator)
                    .FirstOrDefaultAsync(b => b.BusId == busId);

                if (bus == null)
                    return null;

                return MapToBusDTO(bus);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error fetching bus with ID {busId}: {ex.Message}");
            }
        }

        public async Task<IEnumerable<BusDTO>> GetAllBuses()
        {
            try
            {
                var buses = await _context.Buses
                    .Include(b => b.Operator)
                    .ToListAsync();

                return buses.Select(MapToBusDTO);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error fetching all buses: {ex.Message}");
            }
        }

        public async Task<IEnumerable<BusDTO>> GetBusesByOperatorId(int operatorId)
        {
            try
            {
                var buses = await _context.Buses
                    .Where(b => b.OperatorId == operatorId)
                    .Include(b => b.Operator)
                    .ToListAsync();

                return buses.Select(MapToBusDTO);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error fetching buses for operator ID {operatorId}: {ex.Message}");
            }
        }

        public async Task<BusDTO> CreateBus(CreateBusDTO createBusDTO)
        {
            try
            {
                if (!await BusNumberUnique(createBusDTO.BusNumber))
                {
                    throw new InvalidOperationException("The bus number is already in use.");
                }

                var bus = new Bus
                {
                    OperatorId = createBusDTO.OperatorId,
                    BusName = createBusDTO.BusName,
                    BusNumber = createBusDTO.BusNumber,
                    BusType = Enum.Parse<BusTypeEnum>(createBusDTO.BusType, true),
                    TotalSeats = createBusDTO.TotalSeats,
                    Amenities = createBusDTO.Amenities
                };

                await _context.Buses.AddAsync(bus);
                await _context.SaveChangesAsync();

                // Add seats for the bus
                for (int i = 1; i <= createBusDTO.TotalSeats; i++)
                {
                    var seat = new Seat
                    {
                        BusId = bus.BusId,
                        SeatNumber = i.ToString() // Seat numbers as "1", "2", etc.
                    };
                    await _context.Seats.AddAsync(seat);
                }

                await _context.SaveChangesAsync();

                return MapToBusDTO(bus);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error creating bus: {ex.Message}");
            }
        }


        public async Task<BusDTO> UpdateBus(int busId, UpdateBusDTO updateBusDTO)
        {
            try
            {
                var bus = await _context.Buses.Include(b => b.Seats).FirstOrDefaultAsync(b => b.BusId == busId);
                if (bus == null)
                    return null;

                if (!string.IsNullOrWhiteSpace(updateBusDTO.BusNumber) && updateBusDTO.BusNumber != bus.BusNumber)
                {
                    if (!await BusNumberUnique(updateBusDTO.BusNumber))
                    {
                        throw new InvalidOperationException("The bus number is already in use.");
                    }
                    bus.BusNumber = updateBusDTO.BusNumber;
                }

                if (!string.IsNullOrWhiteSpace(updateBusDTO.BusName))
                    bus.BusName = updateBusDTO.BusName;

                if (!string.IsNullOrWhiteSpace(updateBusDTO.BusType))
                    bus.BusType = Enum.Parse<BusTypeEnum>(updateBusDTO.BusType, true);

                if (updateBusDTO.TotalSeats > 0 && updateBusDTO.TotalSeats != bus.TotalSeats)
                {
                    // Update seats if TotalSeats changes
                    var currentSeatCount = bus.Seats.Count;

                    if (updateBusDTO.TotalSeats > currentSeatCount)
                    {
                        // Add new seats
                        for (int i = currentSeatCount + 1; i <= updateBusDTO.TotalSeats; i++)
                        {
                            var seat = new Seat
                            {
                                BusId = bus.BusId,
                                SeatNumber = i.ToString()
                            };
                            await _context.Seats.AddAsync(seat);
                        }
                    }
                    else if (updateBusDTO.TotalSeats < currentSeatCount)
                    {
                        // Remove extra seats
                        var seatsToRemove = bus.Seats.OrderByDescending(s => int.Parse(s.SeatNumber))
                            .Take(currentSeatCount - updateBusDTO.TotalSeats);

                        _context.Seats.RemoveRange(seatsToRemove);
                    }

                    bus.TotalSeats = updateBusDTO.TotalSeats;
                }

                if (!string.IsNullOrWhiteSpace(updateBusDTO.Amenities))
                    bus.Amenities = updateBusDTO.Amenities;

                _context.Buses.Update(bus);
                await _context.SaveChangesAsync();

                return MapToBusDTO(bus);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error updating bus with ID {busId}: {ex.Message}");
            }
        }

        public async Task<BusDTO> DeleteBus(int busId)
        {
            try
            {
                var bus = await _context.Buses.Include(b => b.Seats).FirstOrDefaultAsync(b => b.BusId == busId);
                if (bus == null)
                    return null;

                // Remove related seats
                _context.Seats.RemoveRange(bus.Seats);

                // Remove the bus
                _context.Buses.Remove(bus);
                await _context.SaveChangesAsync();

                return MapToBusDTO(bus);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error deleting bus with ID {busId}: {ex.Message}");
            }
        }

        public async Task<bool> BusNumberUnique(string busNumber)
        {
            try
            {
                return !await _context.Buses.AnyAsync(b => b.BusNumber == busNumber);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error checking bus number uniqueness: {ex.Message}");
            }
        }

        public async Task<IEnumerable<SeatDTO>> GetSeatsByBusId(int busId)
        {
            try
            {
                // Fetch all seats for the given BusId
                var seats = await _context.Seats
                    .Where(s => s.BusId == busId)
                    .ToListAsync();

                // Check if seats were found
                if (seats == null || seats.Count == 0)
                {
                    throw new Exception($"No seats found for BusId: {busId}");
                }

                // Map the Seat entities to SeatDTOs
                return seats.Select(MapToSeatDTO);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error fetching seats for BusId {busId}: {ex.Message}");
            }
        }

        public async Task<IEnumerable<ScheduleSeatDTO>> GetScheduleSeatsByScheduleId(int scheduleId)
        {
            try
            {
                var scheduledSeats = await _context.ScheduleSeats
                    .Where(ss => ss.ScheduleId == scheduleId)
                    .ToListAsync();

                return scheduledSeats.Select(ss => new ScheduleSeatDTO
                {
                    SeatNumber = ss.SeatNumber,
                    BookingId = ss.BookingId,
                    SeatId = ss.SeatId,
                });
            }
            catch (Exception ex)
            {
                throw new Exception($"Error fetching scheduled seats for ScheduleId {scheduleId}: {ex.Message}");
            }
        }


        private static BusDTO MapToBusDTO(Bus bus)
        {
            return new BusDTO
            {
                BusId = bus.BusId,
                OperatorId = bus.OperatorId,
                OperatorName = bus.Operator?.Name,
                BusName = bus.BusName,
                BusNumber = bus.BusNumber,
                BusType = bus.BusType.ToString(),
                TotalSeats = bus.TotalSeats,
                Amenities = bus.Amenities
            };
        }

        private static SeatDTO MapToSeatDTO(Seat seat)
        {
            return new SeatDTO
            {
                SeatId = seat.SeatId,
                BusId = seat.BusId,
                SeatNumber = seat.SeatNumber
            };
        }

    }
}
