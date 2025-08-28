using HouseBrokerApp.Application.Interfaces;
using HouseBrokerApp.Application.Services;
using HouseBrokerApp.Infrastructure.Identity;
using HouseBrokerApp.Infrastructure.Interfaces;
using HouseBrokerApp.Infrastructure.Services;
using HouseBrokerApp.Web.ApiControllers;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace HouseBrokerApp.Tests.ApiControllers
{
    public class AccountApiControllerTests
    {
        private readonly Mock<IUserService> _mockUserService;
        private readonly Mock<ITokenService> _mockTokenService;
        private readonly AccountApiController _controller;

        public AccountApiControllerTests()
        {
            _mockUserService = new Mock<IUserService>();
            _mockTokenService = new Mock<ITokenService>();
            _controller = new AccountApiController(_mockUserService.Object, _mockTokenService.Object);
        }

        [Fact]
        public async Task Register_ReturnsOk_WhenSuccessful()
        {
            _mockUserService.Setup(s => s.RegisterAsync("user1", "test@test.com", "Pass123!", "Broker"))
                            .ReturnsAsync(new { Success = true });

            var result = await _controller.Register("user1", "test@test.com", "Pass123!", "Broker");

            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.NotNull(okResult.Value);
        }

        [Fact]
        public async Task Login_ReturnsOk_WhenValid()
        {
            var user = new ApplicationUser
            {
                Id = Guid.NewGuid(),
                UserName = "user1",
                Email = "test@test.com"
            };

            // Mock UserService to return a user
            _mockUserService.Setup(s => s.LoginAsync("user1", "Pass123"))
                .ReturnsAsync(user);

            // Mock TokenService to return a fake token
            _mockTokenService.Setup(t => t.GenerateTokenAsync(user))
                .ReturnsAsync("fake-jwt-token");

            // Act
            var result = await _controller.Login("user1", "Pass123");

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.NotNull(okResult.Value);
        }


        [Fact]
        public async Task Login_ReturnsUnauthorized_WhenInvalid()
        {
            _mockUserService.Setup(s => s.LoginAsync("wrong", "wrong"))
                            .ReturnsAsync((object?)null);

            var result = await _controller.Login("wrong", "wrong");

            Assert.IsType<UnauthorizedObjectResult>(result);
        }

        [Fact]
        public async Task Logout_ReturnsOk()
        {
            var result = await _controller.Logout();
            Assert.IsType<OkObjectResult>(result);
        }
    }
}
