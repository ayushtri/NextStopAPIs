using Moq;
using NUnit.Framework;
using NextStopAPIs.Controllers;
using NextStopAPIs.DTOs;
using NextStopAPIs.Services;
using Microsoft.AspNetCore.Mvc;
using log4net;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace UnitTesting
{
    [TestFixture]
    public class ScheduleControllerTests
    {
        private Mock<IScheduleService> _scheduleServiceMock;
        private Mock<ILog> _loggerMock;
        private ScheduleController _controller;

        [SetUp]
        public void SetUp()
        {
            _scheduleServiceMock = new Mock<IScheduleService>();
            _loggerMock = new Mock<ILog>();
            _controller = new ScheduleController(_scheduleServiceMock.Object, _loggerMock.Object);
        }

        private ScheduleDTO CreateTestSchedule(int id)
        {
            return new ScheduleDTO
            {
                ScheduleId = id,
                DepartureTime = DateTime.Now,
                ArrivalTime = DateTime.Now.AddHours(2),
                RouteId = 1,
                BusId = 1
            };
        }

        [Test]
        public async Task GetScheduleById_ScheduleNotFound_ReturnsNotFound()
        {
            // Arrange
            int scheduleId = 1;
            _scheduleServiceMock.Setup(s => s.GetScheduleById(scheduleId)).ReturnsAsync((ScheduleDTO)null);

            // Act
            var result = await _controller.GetScheduleById(scheduleId);

            // Assert
            var notFoundResult = result as NotFoundObjectResult;
            Assert.IsNotNull(notFoundResult);
            Assert.AreEqual(404, notFoundResult.StatusCode);
            Assert.AreEqual($"Schedule with ID {scheduleId} not found.", notFoundResult.Value);
        }

        [Test]
        public async Task GetScheduleById_ScheduleFound_ReturnsOk()
        {
            // Arrange
            int scheduleId = 1;
            var schedule = CreateTestSchedule(scheduleId);
            _scheduleServiceMock.Setup(s => s.GetScheduleById(scheduleId)).ReturnsAsync(schedule);

            // Act
            var result = await _controller.GetScheduleById(scheduleId);

            // Assert
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);
            Assert.AreEqual(schedule, okResult.Value);
        }

        [Test]
        public async Task GetAllSchedules_ReturnsOk()
        {
            // Arrange
            var schedules = new List<ScheduleDTO>
            {
                CreateTestSchedule(1),
                CreateTestSchedule(2)
            };
            _scheduleServiceMock.Setup(s => s.GetAllSchedules()).ReturnsAsync(schedules);

            // Act
            var result = await _controller.GetAllSchedules();

            // Assert
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);
            Assert.AreEqual(schedules, okResult.Value);
        }

        [Test]
        public async Task GetSchedulesByRouteId_RouteNotFound_ReturnsNotFound()
        {
            // Arrange
            int routeId = 1;
            _scheduleServiceMock.Setup(s => s.GetSchedulesByRouteId(routeId)).ReturnsAsync((List<ScheduleDTO>)null);

            // Act
            var result = await _controller.GetSchedulesByRouteId(routeId);

            // Assert
            var notFoundResult = result as NotFoundObjectResult;
            Assert.IsNotNull(notFoundResult);
            Assert.AreEqual(404, notFoundResult.StatusCode);
            Assert.AreEqual($"No schedules found for route ID {routeId}.", notFoundResult.Value);
        }

        [Test]
        public async Task GetSchedulesByRouteId_RouteFound_ReturnsOk()
        {
            // Arrange
            int routeId = 1;
            var schedules = new List<ScheduleDTO> { CreateTestSchedule(1) };
            _scheduleServiceMock.Setup(s => s.GetSchedulesByRouteId(routeId)).ReturnsAsync(schedules);

            // Act
            var result = await _controller.GetSchedulesByRouteId(routeId);

            // Assert
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);
            Assert.AreEqual(schedules, okResult.Value);
        }

        [Test]
        public async Task CreateSchedule_Success_ReturnsCreatedAtAction()
        {
            // Arrange
            var createScheduleDTO = new CreateScheduleDTO
            {
                DepartureTime = DateTime.Now,
                ArrivalTime = DateTime.Now.AddHours(2),
                RouteId = 1,
                BusId = 1
            };

            var createdSchedule = new ScheduleDTO
            {
                ScheduleId = 1,
                DepartureTime = createScheduleDTO.DepartureTime,
                ArrivalTime = createScheduleDTO.ArrivalTime,
                RouteId = createScheduleDTO.RouteId,
                BusId = createScheduleDTO.BusId
            };

            _scheduleServiceMock.Setup(s => s.CreateSchedule(createScheduleDTO)).ReturnsAsync(createdSchedule);

            // Act
            var result = await _controller.CreateSchedule(createScheduleDTO);

            // Assert
            var createdResult = result as CreatedAtActionResult;
            Assert.IsNotNull(createdResult);
            Assert.AreEqual(201, createdResult.StatusCode);
            Assert.AreEqual(createdSchedule, createdResult.Value);
        }

        [Test]
        public async Task UpdateSchedule_ScheduleNotFound_ReturnsNotFound()
        {
            // Arrange
            int scheduleId = 1;
            var updateScheduleDTO = new UpdateScheduleDTO
            {
                DepartureTime = DateTime.Now,
                ArrivalTime = DateTime.Now.AddHours(2)
            };
            _scheduleServiceMock.Setup(s => s.UpdateSchedule(scheduleId, updateScheduleDTO)).ReturnsAsync((ScheduleDTO)null);

            // Act
            var result = await _controller.UpdateSchedule(scheduleId, updateScheduleDTO);

            // Assert
            var notFoundResult = result as NotFoundObjectResult;
            Assert.IsNotNull(notFoundResult);
            Assert.AreEqual(404, notFoundResult.StatusCode);
            Assert.AreEqual($"Schedule with ID {scheduleId} not found.", notFoundResult.Value);
        }

        [Test]
        public async Task UpdateSchedule_Success_ReturnsOk()
        {
            // Arrange
            int scheduleId = 1;
            var updateScheduleDTO = new UpdateScheduleDTO
            {
                DepartureTime = DateTime.Now,
                ArrivalTime = DateTime.Now.AddHours(2)
            };
            var updatedSchedule = CreateTestSchedule(scheduleId);
            _scheduleServiceMock.Setup(s => s.UpdateSchedule(scheduleId, updateScheduleDTO)).ReturnsAsync(updatedSchedule);

            // Act
            var result = await _controller.UpdateSchedule(scheduleId, updateScheduleDTO);

            // Assert
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);
            Assert.AreEqual(updatedSchedule, okResult.Value);
        }

        [Test]
        public async Task DeleteSchedule_ScheduleNotFound_ReturnsNotFound()
        {
            // Arrange
            int scheduleId = 1;
            _scheduleServiceMock.Setup(s => s.DeleteSchedule(scheduleId)).ReturnsAsync((ScheduleDTO)null);

            // Act
            var result = await _controller.DeleteSchedule(scheduleId);

            // Assert
            var notFoundResult = result as NotFoundObjectResult;
            Assert.IsNotNull(notFoundResult);
            Assert.AreEqual(404, notFoundResult.StatusCode);

            var response = notFoundResult.Value as ScheduleDeleteResponseDTO;
            Assert.IsNotNull(response);
            Assert.IsFalse(response.Success);
            Assert.AreEqual($"Schedule with ID {scheduleId} not found.", response.Message);
        }

        [Test]
        public async Task DeleteSchedule_Success_ReturnsOk()
        {
            // Arrange
            int scheduleId = 1;
            var schedule = CreateTestSchedule(scheduleId);
            _scheduleServiceMock.Setup(s => s.DeleteSchedule(scheduleId)).ReturnsAsync(schedule);

            // Act
            var result = await _controller.DeleteSchedule(scheduleId);

            // Assert
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);

            var response = okResult.Value as ScheduleDeleteResponseDTO;
            Assert.IsNotNull(response);
            Assert.IsTrue(response.Success);
            Assert.AreEqual($"Schedule with ID {scheduleId} deleted successfully.", response.Message);
        }
    }
}
