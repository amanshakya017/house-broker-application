using HouseBrokerApp.Web.Controllers;
using HouseBrokerApp.Infrastructure.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace HouseBrokerApp.Tests.Controllers
{
    /// <summary>
    /// Tests for <see cref="AccountController"/>.
    /// Covers login, registration, and logout flows with mocked <see cref="IUserService"/>.
    /// </summary>
    public class AccountControllerTests
    {
        private readonly Mock<IUserService> _mockUserService;
        private readonly AccountController _controller;

        public AccountControllerTests()
        {
            _mockUserService = new Mock<IUserService>();
            _controller = new AccountController(_mockUserService.Object);
        }

        [Fact]
        public async Task Login_ValidCredentials_RedirectsToHome()
        {
            // Arrange
            _mockUserService.Setup(s => s.LoginAsync("user", "pass"))
                .ReturnsAsync(new object());

            // Act
            var result = await _controller.Login("user", "pass");

            // Assert
            var redirect = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirect.ActionName);
            Assert.Equal("Home", redirect.ControllerName);
        }

        [Fact]
        public async Task Login_InvalidCredentials_ReturnsViewWithError()
        {
            // Arrange
            _mockUserService.Setup(s => s.LoginAsync("user", "wrong"))
                .ReturnsAsync((object?)null);

            // Act
            var result = await _controller.Login("user", "wrong");

            // Assert
            var view = Assert.IsType<ViewResult>(result);
            Assert.Equal("Invalid username or password", _controller.ViewBag.Error);
        }

        [Fact]
        public async Task Register_Success_RedirectsToLogin()
        {
            _mockUserService.Setup(s => s.RegisterAsync("u", "e", "p", "Broker"))
                .ReturnsAsync(new object());

            var result = await _controller.Register("u", "e", "p", "Broker");

            var redirect = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Login", redirect.ActionName);
        }

        [Fact]
        public async Task Logout_RedirectsToLogin()
        {
            var result = await _controller.Logout();

            var redirect = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Login", redirect.ActionName);
        }
    }
}
