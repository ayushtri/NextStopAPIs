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
    public class FeedbackControllerTest
    {
        private Mock<IFeedbackService> _mockFeedbackService;
        private Mock<ILog> _loggerMock;
        private FeedbackController _controller;

        [SetUp]
        public void Setup()
        {
            _mockFeedbackService = new Mock<IFeedbackService>();
            _loggerMock = new Mock<ILog>();
            _controller = new FeedbackController(_mockFeedbackService.Object, _loggerMock.Object);
        }

        [Test]
        public async Task GetAllFeedbacks_ReturnsOkWithFeedbacks()
        {
            // Arrange
            var feedbacks = new List<FeedbackDTO>
            {
                new FeedbackDTO { FeedbackId = 1, BookingId = 101, Rating = 5, FeedbackText = "Great service!" },
                new FeedbackDTO { FeedbackId = 2, BookingId = 102, Rating = 4, FeedbackText = "Good experience!" }
            };

            _mockFeedbackService.Setup(service => service.GetAllFeedbacks())
                .ReturnsAsync(feedbacks);

            // Act
            var result = await _controller.GetAllFeedbacks();

            // Assert
            Assert.IsInstanceOf<OkObjectResult>(result.Result);
            var okResult = result.Result as OkObjectResult;
            Assert.IsNotNull(okResult);
            var responseFeedbacks = okResult.Value as List<FeedbackResponseDTO>;
            Assert.AreEqual(2, responseFeedbacks.Count);
        }

        [Test]
        public async Task GetFeedbackById_ValidId_ReturnsOkWithFeedback()
        {
            // Arrange
            var feedback = new FeedbackDTO { FeedbackId = 1, BookingId = 101, Rating = 5, FeedbackText = "Great service!" };
            _mockFeedbackService.Setup(service => service.GetFeedbackById(1))
                .ReturnsAsync(feedback);

            // Act
            var result = await _controller.GetFeedbackById(1);

            // Assert
            Assert.IsInstanceOf<OkObjectResult>(result.Result);
            var okResult = result.Result as OkObjectResult;
            Assert.IsNotNull(okResult);
            var responseFeedback = okResult.Value as FeedbackResponseDTO;
            Assert.AreEqual(1, responseFeedback.FeedbackId);
        }

        [Test]
        public async Task GetFeedbackById_InvalidId_ReturnsNotFound()
        {
            // Arrange
            _mockFeedbackService.Setup(service => service.GetFeedbackById(99))
                .ReturnsAsync((FeedbackDTO)null);

            // Act
            var result = await _controller.GetFeedbackById(99);

            // Assert
            Assert.IsInstanceOf<NotFoundObjectResult>(result.Result);
        }

        [Test]
        public async Task AddFeedback_ValidFeedback_ReturnsCreatedAtAction()
        {
            // Arrange
            var feedbackDTO = new AddFeedbackDTO { BookingId = 101, Rating = 5, FeedbackText = "Great service!" };
            var createdFeedbackDTO = new FeedbackDTO { FeedbackId = 1, BookingId = 101, Rating = 5, FeedbackText = "Great service!" };

            _mockFeedbackService.Setup(service => service.AddFeedback(It.IsAny<AddFeedbackDTO>()))
                .ReturnsAsync(createdFeedbackDTO); 

            _mockFeedbackService.Setup(service => service.GetFeedbackById(It.IsAny<int>()))
                .ReturnsAsync(createdFeedbackDTO); 

            // Act
            var result = await _controller.AddFeedback(feedbackDTO);

            // Assert
            Assert.IsInstanceOf<CreatedAtActionResult>(result.Result);
            var createdAtResult = result.Result as CreatedAtActionResult;
            Assert.IsNotNull(createdAtResult);

            var responseFeedback = createdAtResult.Value as FeedbackResponseDTO;
            Assert.AreEqual(1, responseFeedback.FeedbackId);  
            Assert.AreEqual(101, responseFeedback.BookingId); 
            Assert.AreEqual(5, responseFeedback.Rating);     
            Assert.AreEqual("Great service!", responseFeedback.FeedbackText); 
        }

    }
}
