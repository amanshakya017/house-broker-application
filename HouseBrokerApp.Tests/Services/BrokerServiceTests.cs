using HouseBrokerApp.Application.Services;
using HouseBrokerApp.Core.Entities;
using HouseBrokerApp.Core.Interfaces;
using Moq;
using System.Linq.Expressions;

namespace HouseBrokerApp.Tests.Services
{
    public class BrokerServiceTests
    {
        private readonly Mock<IUnitOfWork> _mockUow;
        private readonly BrokerService _brokerService;

        public BrokerServiceTests()
        {
            _mockUow = new Mock<IUnitOfWork>();
            _brokerService = new BrokerService(_mockUow.Object);
        }

        [Theory]
        [InlineData(4000000, 80000)]
        [InlineData(6000000, 105000)]
        [InlineData(15000000, 225000)]
        public async Task CalculateCommission_WorksCorrectly(decimal price, decimal expectedCommission)
        {
            // Arrange - mock commission rules
            var rules = new List<CommissionRule>
            {
                new CommissionRule { MinPrice = 0, MaxPrice = 5000000, Rate = 0.02m },
                new CommissionRule { MinPrice = 5000001, MaxPrice = 10000000, Rate = 0.0175m },
                new CommissionRule { MinPrice = 10000001, MaxPrice = 20000000, Rate = 0.015m }
            };

            var repoMock = new Mock<IRepository<CommissionRule>>();
            repoMock.Setup(r => r.GetAllAsync()).ReturnsAsync(rules);

            _mockUow.Setup(u => u.Repository<CommissionRule>()).Returns(repoMock.Object);

            // Act
            var result = await _brokerService.CalculateCommissionAsync(price);

            // Assert
            Assert.Equal(expectedCommission, result);
        }

        [Fact]
        public async Task GetTotalCommissionAsync_ReturnsCorrectSum()
        {
            var brokerId = Guid.NewGuid();
            var listings = new List<PropertyListing>
            {
                new PropertyListing { BrokerId = brokerId, Commission = 10000 },
                new PropertyListing { BrokerId = brokerId, Commission = 15000 }
            };

            var repoMock = new Mock<IRepository<PropertyListing>>();

            // FIX: use Expression instead of Func
            repoMock.Setup(r => r.FindAsync(It.IsAny<Expression<Func<PropertyListing, bool>>>()))
                .ReturnsAsync(listings);

            _mockUow.Setup(u => u.Repository<PropertyListing>()).Returns(repoMock.Object);

            // Act
            var total = await _brokerService.GetTotalCommissionAsync(brokerId);

            // Assert
            Assert.Equal(25000m, total);
        }
    }
}
