using HouseBrokerApp.Application.DTOs;
using HouseBrokerApp.Application.Interfaces;
using HouseBrokerApp.Core.Enums;
using HouseBrokerApp.Infrastructure.Identity;
using HouseBrokerApp.Web.ApiControllers;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace HouseBrokerApp.Tests.ApiControllers
{
    public class ListingsApiControllerTests
    {
        private readonly Mock<IListingService> _mockListingService;
        private readonly Mock<UserManager<ApplicationUser>> _mockUserManager;
        private readonly ListingsApiController _controller;

        public ListingsApiControllerTests()
        {
            _mockListingService = new Mock<IListingService>();
            _mockUserManager = new Mock<UserManager<ApplicationUser>>();
            _controller = new ListingsApiController(_mockListingService.Object, _mockUserManager.Object);
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
            var dto = new PropertyListingDto { Id = Guid.NewGuid(), PropertyType = PropertyType.Apartment, Location = "KTM", Price = 5000000 };

            var result = await _controller.Create(dto);

            var created = Assert.IsType<CreatedAtActionResult>(result);
            Assert.Equal("GetById", created.ActionName);
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
