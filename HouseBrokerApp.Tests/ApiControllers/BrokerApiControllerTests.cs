using HouseBrokerApp.Application.Interfaces;
using HouseBrokerApp.Core.Entities;
using HouseBrokerApp.Web.ApiControllers;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace HouseBrokerApp.Tests.ApiControllers
{
    public class BrokerApiControllerTests
    {
        private readonly Mock<IBrokerService> _mockBrokerService;
        private readonly BrokerApiController _controller;

        public BrokerApiControllerTests()
        {
            _mockBrokerService = new Mock<IBrokerService>();
            _controller = new BrokerApiController(_mockBrokerService.Object);
        }

        [Fact]
        public async Task GetBrokerListings_ReturnsOk()
        {
            var brokerId = Guid.NewGuid();
            var listings = new List<PropertyListing>
            {
                new PropertyListing { Id = Guid.NewGuid(), BrokerId = brokerId, Price = 500000 },
                new PropertyListing { Id = Guid.NewGuid(), BrokerId = brokerId, Price = 1000000 }
            };

            _mockBrokerService.Setup(s => s.GetBrokerListingsAsync(brokerId))
                .ReturnsAsync(listings);

            var result = await _controller.GetBrokerListings(brokerId);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedListings = Assert.IsAssignableFrom<IEnumerable<PropertyListing>>(okResult.Value);
            Assert.Equal(2, returnedListings.Count());
        }

        [Fact]
        public async Task GetTotalCommission_ReturnsOk()
        {
            var brokerId = Guid.NewGuid();
            decimal expected = 25000m;

            _mockBrokerService.Setup(s => s.GetTotalCommissionAsync(brokerId))
                .ReturnsAsync(expected);

            var result = await _controller.GetTotalCommission(brokerId);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var total = Assert.IsType<decimal>(okResult.Value);
            Assert.Equal(expected, total); // fixed decimal literal
        }

        [Fact]
        public async Task CalculateCommission_ReturnsOk()
        {
            decimal price = 5000000m;
            decimal expectedCommission = 100000m;

            _mockBrokerService.Setup(s => s.CalculateCommissionAsync(price))
                .ReturnsAsync(expectedCommission);

            var result = await _controller.CalculateCommission(price);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var commission = Assert.IsType<decimal>(okResult.Value);
            Assert.Equal(expectedCommission, commission); // fixed decimal literal
        }
    }
}
