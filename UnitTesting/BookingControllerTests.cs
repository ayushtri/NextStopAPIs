using Moq;
using NUnit.Framework;
using Microsoft.AspNetCore.Mvc;
using NextStopAPIs.Controllers;
using NextStopAPIs.DTOs;
using NextStopAPIs.Services;
using System.Collections.Generic;
using System.Threading.Tasks;
using log4net;

namespace UnitTesting
{
    [TestFixture]
    public class BookingControllerTests
    {
        private Mock<IBookingService> _bookingServiceMock;
        private Mock<ILog> _loggerMock;
        private BookingController _controller;

        [SetUp]
        public void SetUp()
        {
            _bookingServiceMock = new Mock<IBookingService>();
            _loggerMock = new Mock<ILog>();
            _controller = new BookingController(_bookingServiceMock.Object, _loggerMock.Object);
        }

        [Test]
        public async Task SearchBus_Success_ReturnsOk()
        {
            // Arrange
            var searchBusDto = new SearchBusDTO { Origin = "A", Destination = "B", TravelDate = System.DateTime.Now };
            var buses = new List<BusSearchResultDTO>
            {
                new BusSearchResultDTO { ScheduleId = 1, BusId = 1, BusName = "Bus A", Origin = "A", Destination = "B", DepartureTime = System.DateTime.Now, AvailableSeats = 20 }
            };
            _bookingServiceMock.Setup(s => s.SearchBus(searchBusDto)).ReturnsAsync(buses);

            // Act
            var result = await _controller.SearchBus(searchBusDto);

            // Assert
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);
            Assert.AreEqual(buses, okResult.Value);
        }

        [Test]
        public async Task BookTicket_Success_ReturnsCreated()
        {
            // Arrange
            var bookTicketDto = new BookTicketDTO { UserId = 1, ScheduleId = 1, SeatNumbers = new List<string> { "A1", "A2" } };
            var booking = new BookingDTO
            {
                BookingId = 1,
                UserId = 1,
                ScheduleId = 1,
                ReservedSeats = new List<string> { "A1", "A2" },
                TotalFare = 200.0m,
                Status = "confirmed",
                BookingDate = System.DateTime.Now
            };
            _bookingServiceMock.Setup(s => s.BookTicket(bookTicketDto)).ReturnsAsync(booking);

            // Act
            var result = await _controller.BookTicket(bookTicketDto);

            // Assert
            var createdResult = result as CreatedAtActionResult;
            Assert.IsNotNull(createdResult);
            Assert.AreEqual(201, createdResult.StatusCode);
            Assert.AreEqual(booking, createdResult.Value);
        }

        [Test]
        public async Task CancelBooking_Success_ReturnsOk()
        {
            // Arrange
            var cancelBookingDto = new CancelBookingDTO { BookingId = 1 };
            _bookingServiceMock.Setup(s => s.CancelBooking(cancelBookingDto.BookingId)).ReturnsAsync(true);

            // Act
            var result = await _controller.CancelBooking(cancelBookingDto);

            // Assert
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);
            var response = okResult.Value as CancelBookingResponseDTO;
            Assert.IsTrue(response.Success);
            Assert.AreEqual("Booking cancelled successfully.", response.Message);
        }

        [Test]
        public async Task CancelBooking_Fail_ReturnsNotFound()
        {
            // Arrange
            var cancelBookingDto = new CancelBookingDTO { BookingId = 1 };
            _bookingServiceMock.Setup(s => s.CancelBooking(cancelBookingDto.BookingId)).ReturnsAsync(false);

            // Act
            var result = await _controller.CancelBooking(cancelBookingDto);

            // Assert
            var notFoundResult = result as NotFoundObjectResult;
            Assert.IsNotNull(notFoundResult);
            Assert.AreEqual(404, notFoundResult.StatusCode);
        }

        [Test]
        public async Task ViewBookingsByUserId_NoBookings_ReturnsNotFound()
        {
            // Arrange
            int userId = 1;
            _bookingServiceMock.Setup(s => s.GetBookingsByUserId(userId)).ReturnsAsync(new List<BookingDTO>());

            // Act
            var result = await _controller.ViewBookingsByUserId(userId);

            // Assert
            var notFoundResult = result as NotFoundObjectResult;
            Assert.IsNotNull(notFoundResult);
            Assert.AreEqual(404, notFoundResult.StatusCode);
            Assert.AreEqual("No bookings found for this user.", notFoundResult.Value);
        }

        [Test]
        public async Task ViewBookingByBookingId_Success_ReturnsOk()
        {
            // Arrange
            int bookingId = 1;
            var booking = new BookingDTO
            {
                BookingId = bookingId,
                UserId = 1,
                ScheduleId = 1,
                ReservedSeats = new List<string> { "A1", "A2" },
                TotalFare = 200.0m,
                Status = "confirmed",
                BookingDate = System.DateTime.Now
            };
            _bookingServiceMock.Setup(s => s.GetBookingById(bookingId)).ReturnsAsync(booking);

            // Act
            var result = await _controller.ViewBookingByBookingId(bookingId);

            // Assert
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);
            Assert.AreEqual(booking, okResult.Value);
        }

        [Test]
        public async Task ViewBookingByBookingId_Fail_ReturnsNotFound()
        {
            // Arrange
            int bookingId = 1;
            _bookingServiceMock.Setup(s => s.GetBookingById(bookingId)).ReturnsAsync((BookingDTO)null);

            // Act
            var result = await _controller.ViewBookingByBookingId(bookingId);

            // Assert
            var notFoundResult = result as NotFoundObjectResult;
            Assert.IsNotNull(notFoundResult);
            Assert.AreEqual(404, notFoundResult.StatusCode);
        }
    }
}
