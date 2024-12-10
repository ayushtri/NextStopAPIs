using Microsoft.EntityFrameworkCore;
using NextStopAPIs.Data;
using NextStopAPIs.DTOs;
using NextStopAPIs.Models;

namespace NextStopAPIs.Services
{
    public class ScheduleService : IScheduleService
    {
        private readonly NextStopDbContext _context;

        public ScheduleService(NextStopDbContext context)
        {
            _context = context;
        }

        public async Task<ScheduleDTO> GetScheduleById(int scheduleId)
        {
            try
            {
                var schedule = await _context.Schedules
                    .Include(s => s.Bus)
                        .ThenInclude(b => b.Operator)
                    .Include(s => s.Route)
                    .Include(s => s.ScheduleSeats)
                        .ThenInclude(ss => ss.Seat)
                    .FirstOrDefaultAsync(s => s.ScheduleId == scheduleId);

                if (schedule == null)
                    return null;

                return MapToScheduleDTO(schedule);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error fetching schedule with ID {scheduleId}: {ex.Message}");
            }
        }

        public async Task<IEnumerable<ScheduleDTO>> GetAllSchedules()
        {
            try
            {
                var schedules = await _context.Schedules
                    .Include(s => s.Bus)
                        .ThenInclude(b => b.Operator)
                    .Include(s => s.Route)
                    .Include(s => s.ScheduleSeats)
                        .ThenInclude(ss => ss.Seat)
                    .ToListAsync();

                return schedules.Select(MapToScheduleDTO);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error fetching all schedules: {ex.Message}");
            }
        }

        public async Task<IEnumerable<ScheduleDTO>> GetSchedulesByRouteId(int routeId)
        {
            try
            {
                var schedules = await _context.Schedules
                    .Where(s => s.RouteId == routeId)
                    .Include(s => s.Bus)
                        .ThenInclude(b => b.Operator)
                    .Include(s => s.Route)
                    .Include(s => s.ScheduleSeats)
                        .ThenInclude(ss => ss.Seat)
                    .ToListAsync();

                return schedules.Select(MapToScheduleDTO);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error fetching schedules for route ID {routeId}: {ex.Message}");
            }
        }

        public async Task<IEnumerable<ScheduleDTO>> GetSchedulesByBusId(int busId)
        {
            try
            {
                var schedules = await _context.Schedules
                    .Where(s => s.BusId == busId)
                    .Include(s => s.Bus)
                        .ThenInclude (b => b.Operator)
                    .Include(s => s.Route)
                    .Include(s => s.ScheduleSeats)
                        .ThenInclude(ss => ss.Seat)
                    .ToListAsync();

                return schedules.Select(MapToScheduleDTO);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error fetching schedules for bus ID {busId}: {ex.Message}");
            }
        }

        public async Task<ScheduleDTO> CreateSchedule(CreateScheduleDTO createScheduleDTO)
        {
            try
            {
                var schedule = new Schedule
                {
                    BusId = createScheduleDTO.BusId,
                    RouteId = createScheduleDTO.RouteId,
                    DepartureTime = createScheduleDTO.DepartureTime,
                    ArrivalTime = createScheduleDTO.ArrivalTime,
                    Fare = createScheduleDTO.Fare,
                    Date = createScheduleDTO.Date
                };

                await _context.Schedules.AddAsync(schedule);
                await _context.SaveChangesAsync();

                return MapToScheduleDTO(schedule);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error creating schedule: {ex.Message}");
            }
        }

        public async Task<ScheduleDTO> UpdateSchedule(int scheduleId, UpdateScheduleDTO updateScheduleDTO)
        {
            try
            {
                var existingSchedule = await _context.Schedules
                    .Include(s => s.ScheduleSeats)
                    .FirstOrDefaultAsync(s => s.ScheduleId == scheduleId);

                if (existingSchedule == null)
                {
                    return null;
                }

                if (updateScheduleDTO.BusId.HasValue)
                    existingSchedule.BusId = updateScheduleDTO.BusId.Value;

                if (updateScheduleDTO.RouteId.HasValue)
                    existingSchedule.RouteId = updateScheduleDTO.RouteId.Value;

                if (updateScheduleDTO.DepartureTime.HasValue)
                    existingSchedule.DepartureTime = updateScheduleDTO.DepartureTime.Value;

                if (updateScheduleDTO.ArrivalTime.HasValue)
                    existingSchedule.ArrivalTime = updateScheduleDTO.ArrivalTime.Value;

                if (updateScheduleDTO.Fare.HasValue)
                    existingSchedule.Fare = updateScheduleDTO.Fare.Value;

                if (updateScheduleDTO.Date.HasValue)
                    existingSchedule.Date = updateScheduleDTO.Date.Value;

                _context.Schedules.Update(existingSchedule);
                await _context.SaveChangesAsync();

                return MapToScheduleDTO(existingSchedule);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error updating schedule with ID {scheduleId}: {ex.Message}");
            }
        }

        public async Task<ScheduleDTO> DeleteSchedule(int scheduleId)
        {
            try
            {
                var schedule = await _context.Schedules
                    .Include(s => s.ScheduleSeats)
                    .FirstOrDefaultAsync(s => s.ScheduleId == scheduleId);

                if (schedule == null)
                {
                    return null;
                }

                _context.Schedules.Remove(schedule);
                await _context.SaveChangesAsync();

                return MapToScheduleDTO(schedule);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error deleting schedule with ID {scheduleId}: {ex.Message}");
            }
        }

        private static ScheduleDTO MapToScheduleDTO(Schedule schedule)
        {
            return new ScheduleDTO
            {
                ScheduleId = schedule.ScheduleId,
                BusId = schedule.BusId,
                BusNumber = schedule.Bus != null ? schedule.Bus.BusNumber : "Unknown Bus Number", // Map BusNumber
                BusName = schedule.Bus != null ? schedule.Bus.BusName : "Unknown Bus",
                OperatorId = schedule.Bus?.OperatorId ?? 0, // Map OperatorId
                OperatorName = schedule.Bus?.Operator?.Name ?? "Unknown Operator", // Map OperatorName
                RouteId = schedule.RouteId,
                Origin = schedule.Route != null ? schedule.Route.Origin : "Unknown Origin",
                Destination = schedule.Route != null ? schedule.Route.Destination : "Unknown Destination",
                DepartureTime = schedule.DepartureTime,
                ArrivalTime = schedule.ArrivalTime,
                Fare = schedule.Fare,
                Date = schedule.Date,
                Seats = schedule.ScheduleSeats != null
                    ? schedule.ScheduleSeats.Select(ss => new ScheduleSeatDTO
                    {
                        SeatId = ss.SeatId,
                        SeatNumber = ss.SeatNumber
                    }).ToList()
                    : new List<ScheduleSeatDTO>()
            };
        }


        public async Task<IEnumerable<ScheduleDTO>> GetSchedulesByOperatorId(int operatorId)
        {
            try
            {
                var schedules = await _context.Schedules
                    .Include(s => s.Bus) // Include related bus information
                    .ThenInclude(b => b.Operator) // Ensure operator data is loaded
                    .Include(s => s.Route) // Include related route information
                    .Include(s => s.ScheduleSeats)
                        .ThenInclude(ss => ss.Seat) // Include seat information
                    .Where(s => s.Bus.OperatorId == operatorId) // Filter schedules by operator ID
                    .ToListAsync();

                return schedules.Select(MapToScheduleDTO); // Map to DTO
            }
            catch (Exception ex)
            {
                throw new Exception($"Error fetching schedules for operator ID {operatorId}: {ex.Message}");
            }
        }

    }
}
