using HouseBrokerApp.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace HouseBrokerApp.Infrastructure.Services
{
    /// <summary>
    /// Service to generate JWT tokens for authenticated users.
    /// Supports both Base64-encoded and plain UTF8 JWT keys.
    /// </summary>
    public class TokenService : ITokenService
    {
        private readonly IConfiguration _config;
        private readonly UserManager<ApplicationUser> _userManager;

        public TokenService(IConfiguration config, UserManager<ApplicationUser> userManager)
        {
            _config = config;
            _userManager = userManager;
        }

        /// <summary>
        /// Generates a signed JWT token for the given user.
        /// </summary>
        /// <param name="user">The authenticated application user.</param>
        /// <returns>A signed JWT token string.</returns>
        public async Task<string> GenerateTokenAsync(ApplicationUser user)
        {
            var keyString = _config["Jwt:Key"];
            if (string.IsNullOrEmpty(keyString))
                throw new InvalidOperationException("JWT Key is missing from configuration.");

            // Try Base64 first, fallback to UTF8
            byte[] keyBytes = Convert.FromBase64String(keyString);

            if (keyBytes.Length < 32) // Must be >= 256 bits
                throw new InvalidOperationException("JWT Key must be at least 256 bits (32 bytes). Please update appsettings.json.");

            // Claims
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.UniqueName, user.UserName ?? string.Empty),
                new Claim(JwtRegisteredClaimNames.Email, user.Email ?? string.Empty)
            };

            var roles = await _userManager.GetRolesAsync(user);
            claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

            var key = new SymmetricSecurityKey(keyBytes);
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"] ?? _config["Jwt:Issuer"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(int.Parse(_config["Jwt:ExpireHours"] ?? "2")),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
