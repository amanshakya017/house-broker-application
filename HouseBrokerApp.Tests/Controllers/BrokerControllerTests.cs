using HouseBrokerApp.Application.Interfaces;
using HouseBrokerApp.Core.Entities;
using HouseBrokerApp.Core.Enums;
using HouseBrokerApp.Infrastructure.Identity;
using HouseBrokerApp.Web.Controllers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Security.Claims;

namespace HouseBrokerApp.Tests.Controllers
{
    /// <summary>
    /// Unit tests for <see cref="BrokerController"/>.
    /// Validates broker dashboard behavior with mock <see cref="IBrokerService"/> and <see cref="UserManager{TUser}"/>.
    /// </summary>
    public class BrokerControllerTests
    {
        private readonly Mock<IBrokerService> _mockBrokerService;
        private readonly Mock<UserManager<ApplicationUser>> _mockUserManager;
        private readonly BrokerController _controller;

        public BrokerControllerTests()
        {
            _mockBrokerService = new Mock<IBrokerService>();

            var store = new Mock<IUserStore<ApplicationUser>>();
            _mockUserManager = new Mock<UserManager<ApplicationUser>>(
                store.Object, null, null, null, null, null, null, null, null);

            _controller = new BrokerController(_mockBrokerService.Object, _mockUserManager.Object);

            // Fake HttpContext with logged-in user
            var user = new ClaimsPrincipal(new ClaimsIdentity(
                new Claim[] { new Claim(ClaimTypes.NameIdentifier, Guid.NewGuid().ToString()) }, "mock"));

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = user }
            };
        }

        [Fact]
        public async Task Dashboard_UserNotFound_ReturnsUnauthorized()
        {
            // Arrange
            _mockUserManager.Setup(m => m.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
                .ReturnsAsync((ApplicationUser?)null);

            // Act
            var result = await _controller.Dashboard();

            // Assert
            Assert.IsType<UnauthorizedResult>(result);
        }

        [Fact]
        public async Task Dashboard_ReturnsViewWithListingsAndCommission()
        {
            // Arrange
            var user = new ApplicationUser { Id = Guid.NewGuid() };
            _mockUserManager.Setup(m => m.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
                .ReturnsAsync(user);

            // Return a real PropertyListing list with enum PropertyType
            var listings = new List<PropertyListing>
                            {
                                new PropertyListing
                                {
                                    PropertyType = PropertyType.House,
                                    Location = "KTM",
                                    Price = 5000000,
                                    BrokerId = user.Id,
                                    Commission = 100000
                                }
                            };

            _mockBrokerService.Setup(s => s.GetBrokerListingsAsync(user.Id))
                .ReturnsAsync(listings);

            _mockBrokerService.Setup(s => s.GetTotalCommissionAsync(user.Id))
                .ReturnsAsync(50000);

            // Act
            var result = await _controller.Dashboard();

            // Assert
            var view = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<IEnumerable<PropertyListing>>(view.Model);

            Assert.Single(model);
            Assert.Equal(PropertyType.House, model.First().PropertyType);
            Assert.Equal(50000, _controller.ViewBag.TotalCommission);
        }

    }
}
