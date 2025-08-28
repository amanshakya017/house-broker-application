using HouseBrokerApp.Application.Services;
using HouseBrokerApp.Infrastructure.Identity;
using HouseBrokerApp.Infrastructure.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Moq;
using System.Text;
using Xunit;

namespace HouseBrokerApp.Tests.Services
{
    /// <summary>
    /// Unit tests for <see cref="TokenService"/>.
    /// Verifies token generation logic with mock user data and roles.
    /// </summary>
    public class TokenServiceTests
    {
        private readonly Mock<UserManager<ApplicationUser>> _mockUserManager;
        private readonly TokenService _tokenService;

        public TokenServiceTests()
        {
            // Setup fake user manager
            var store = new Mock<IUserStore<ApplicationUser>>();
            _mockUserManager = new Mock<UserManager<ApplicationUser>>(
                store.Object, null, null, null, null, null, null, null, null);

            // Setup configuration with JWT secret
            var inMemorySettings = new Dictionary<string, string>
            {
                {"Jwt:Key", "super_secret_test_key_12345"},
                {"Jwt:Issuer", "test-issuer"}
            };
            var config = new ConfigurationBuilder()
                .AddInMemoryCollection(inMemorySettings!)
                .Build();

            _tokenService = new TokenService(config, _mockUserManager.Object);
        }

        [Fact]
        public async Task GenerateTokenAsync_ReturnsValidJwtString()
        {
            // Arrange
            var user = new ApplicationUser
            {
                Id = Guid.NewGuid(),
                UserName = "testuser",
                Email = "test@example.com"
            };

            _mockUserManager.Setup(m => m.GetRolesAsync(user))
                .ReturnsAsync(new List<string> { "Broker" });

            // Act
            var token = await _tokenService.GenerateTokenAsync(user);

            // Assert
            Assert.False(string.IsNullOrEmpty(token));
            Assert.Contains("eyJ", token); // JWTs start with "eyJ"
        }
    }
}
