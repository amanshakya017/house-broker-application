using HouseBrokerApp.Infrastructure.Identity;
using HouseBrokerApp.Infrastructure.Services;
using Microsoft.AspNetCore.Identity;
using Moq;

namespace HouseBrokerApp.Tests
{
    public class UserServiceTests
    {
        private readonly Mock<UserManager<ApplicationUser>> _mockUserManager;
        private readonly Mock<SignInManager<ApplicationUser>> _mockSignInManager;
        private readonly UserService _userService;

        public UserServiceTests()
        {
            // Mock UserManager
            var store = new Mock<IUserStore<ApplicationUser>>();
            _mockUserManager = new Mock<UserManager<ApplicationUser>>(
                store.Object, null, null, null, null, null, null, null, null
            );

            // Mock SignInManager
            var contextAccessor = new Mock<Microsoft.AspNetCore.Http.IHttpContextAccessor>();
            var claimsFactory = new Mock<IUserClaimsPrincipalFactory<ApplicationUser>>();
            _mockSignInManager = new Mock<SignInManager<ApplicationUser>>(
                _mockUserManager.Object,
                contextAccessor.Object,
                claimsFactory.Object,
                null,
                null,
                null,
                null
            );

            _userService = new UserService(_mockUserManager.Object, _mockSignInManager.Object);
        }

        [Fact]
        public async Task RegisterAsync_CreatesUserAndAddsRole()
        {
            // Arrange
            var username = "testuser";
            var email = "test@example.com";
            var password = "P@ssw0rd";
            var role = "Seeker";

            _mockUserManager.Setup(m => m.CreateAsync(It.IsAny<ApplicationUser>(), password))
                            .ReturnsAsync(IdentityResult.Success);
            _mockUserManager.Setup(m => m.AddToRoleAsync(It.IsAny<ApplicationUser>(), role))
                            .ReturnsAsync(IdentityResult.Success);

            // Act
            var result = await _userService.RegisterAsync(username, email, password, role);

            // Assert
            Assert.NotNull(result);
            var user = Assert.IsType<ApplicationUser>(result);
            Assert.Equal(username, user.UserName);
            Assert.Equal(email, user.Email);
        }

        [Fact]
        public async Task LoginAsync_ReturnsUser_WhenSuccessful()
        {
            // Arrange
            var username = "testuser";
            var password = "P@ssw0rd";

            var user = new ApplicationUser { UserName = username };

            _mockSignInManager.Setup(m => m.PasswordSignInAsync(username, password, false, false))
                              .ReturnsAsync(SignInResult.Success);
            _mockUserManager.Setup(m => m.FindByNameAsync(username))
                            .ReturnsAsync(user);

            // Act
            var result = await _userService.LoginAsync(username, password);

            // Assert
            Assert.NotNull(result);
            var loggedInUser = Assert.IsType<ApplicationUser>(result);
            Assert.Equal(username, loggedInUser.UserName);
        }

        [Fact]
        public async Task LoginAsync_ReturnsNull_WhenFailed()
        {
            // Arrange
            var username = "testuser";
            var password = "WrongPassword";

            _mockSignInManager.Setup(m => m.PasswordSignInAsync(username, password, false, false))
                              .ReturnsAsync(SignInResult.Failed);

            // Act
            var result = await _userService.LoginAsync(username, password);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task LogoutAsync_CallsSignOut()
        {
            // Arrange
            _mockSignInManager.Setup(m => m.SignOutAsync()).Returns(Task.CompletedTask);

            // Act
            await _userService.LogoutAsync();

            // Assert
            _mockSignInManager.Verify(m => m.SignOutAsync(), Times.Once);
        }
    }
}
