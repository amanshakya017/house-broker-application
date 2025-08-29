using HouseBrokerApp.Application.DTOs;
using HouseBrokerApp.Application.Interfaces;
using HouseBrokerApp.Core.Enums;
using HouseBrokerApp.Infrastructure.Identity;
using HouseBrokerApp.Web.ApiControllers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Security.Claims;

namespace HouseBrokerApp.Tests.ApiControllers
{
    public class ListingsApiControllerTests
    {
        private readonly Mock<IListingService> _mockListingService;
        private readonly Mock<UserManager<ApplicationUser>> _mockUserManager;
        private readonly Mock<IFileStorage> _mockFileStorage;
        private readonly ListingsApiController _controller;
        public ListingsApiControllerTests()
        {
            _mockListingService = new Mock<IListingService>();

            var store = new Mock<IUserStore<ApplicationUser>>();
            _mockUserManager = new Mock<UserManager<ApplicationUser>>(
                store.Object, null, null, null, null, null, null, null, null);

            _mockFileStorage = new Mock<IFileStorage>();

            _controller = new ListingsApiController(
                _mockListingService.Object,
                _mockUserManager.Object,
                _mockFileStorage.Object
            );
        }

        // Helper to safely create UserManager mock
        private static Mock<UserManager<ApplicationUser>> MockUserManager()
        {
            var store = new Mock<IUserStore<ApplicationUser>>();
            return new Mock<UserManager<ApplicationUser>>(
                store.Object,
                null, null, null, null, null, null, null, null
            );
        }

        [Fact]
        public async Task GetAll_ReturnsOk_WithListings()
        {
            var listings = new List<PropertyListingDto> { new() { Id = Guid.NewGuid(), PropertyType = PropertyType.House } };
            _mockListingService.Setup(s => s.GetAllAsync()).ReturnsAsync(listings);

            var result = await _controller.GetAll();

            var ok = Assert.IsType<OkObjectResult>(result);
            var value = Assert.IsAssignableFrom<IEnumerable<PropertyListingDto>>(ok.Value);
            Assert.Single(value);
        }

        [Fact]
        public async Task GetById_ReturnsNotFound_WhenMissing()
        {
            _mockListingService.Setup(s => s.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((PropertyListingDto?)null);

            var result = await _controller.GetById(Guid.NewGuid());

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Create_ReturnsCreatedAtAction()
        {
            // Arrange
            var stream = new MemoryStream(new byte[] { 1, 2, 3 });
            var formFile = new FormFile(stream, 0, stream.Length, "file", "test.jpg");

            var dto = new PropertyListingDto
            {
                PropertyType = PropertyType.House,
                Location = "Kathmandu",
                Price = 5000000,
                Features = "3BHK",
                Description = "Nice house",
                ImageFile = formFile
            };

            _mockFileStorage.Setup(f => f.SaveFileAsync(It.IsAny<IFormFile>()))
                .ReturnsAsync("/img/test.jpg");

            _mockListingService.Setup(s => s.AddAsync(It.IsAny<PropertyListingDto>()))
                .Returns(Task.CompletedTask);

            _mockUserManager.Setup(m => m.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
                .ReturnsAsync(new ApplicationUser { Id = Guid.NewGuid() });

            // Act
            var result = await _controller.Create(dto);

            // Assert
            var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result);
            var returnDto = Assert.IsType<PropertyListingDto>(createdAtActionResult.Value);

            Assert.Equal("/img/test.jpg", returnDto.ImageUrl);
            _mockListingService.Verify(s => s.AddAsync(It.IsAny<PropertyListingDto>()), Times.Once);
            _mockFileStorage.Verify(f => f.SaveFileAsync(It.IsAny<IFormFile>()), Times.Once);
        }


        [Fact]
        public async Task Search_ReturnsOk_WithResults()
        {
            var listings = new List<PropertyListingDto> { new() { Id = Guid.NewGuid(), PropertyType = PropertyType.Land } };
            _mockListingService.Setup(s => s.SearchAsync("KTM", null, null, null)).ReturnsAsync(listings);

            var result = await _controller.Search("KTM", null, null, null);

            var ok = Assert.IsType<OkObjectResult>(result);
            var value = Assert.IsAssignableFrom<IEnumerable<PropertyListingDto>>(ok.Value);
            Assert.Single(value);
        }
    }
}
