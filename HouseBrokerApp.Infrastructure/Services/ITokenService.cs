using HouseBrokerApp.Infrastructure.Identity;

namespace HouseBrokerApp.Infrastructure.Services
{
    /// <summary>
    /// Contract for generating JWT tokens for authenticated users.
    /// </summary>
    public interface ITokenService
    {
        /// <summary>
        /// Generates a JWT token for the given authenticated user.
        /// </summary>
        /// <param name="user">The authenticated application user.</param>
        /// <returns>A signed JWT token string.</returns>
        Task<string> GenerateTokenAsync(ApplicationUser user);
    }
}
