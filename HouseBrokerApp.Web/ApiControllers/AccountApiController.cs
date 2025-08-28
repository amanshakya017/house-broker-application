using HouseBrokerApp.Infrastructure.Identity;
using HouseBrokerApp.Infrastructure.Interfaces;
using HouseBrokerApp.Infrastructure.Services;
using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography;

namespace HouseBrokerApp.Web.ApiControllers
{
    [Route("api/account")]
    [ApiController]
    public class AccountApiController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly ITokenService _tokenService;

        public AccountApiController(IUserService userService, ITokenService tokenService)
        {
            _userService = userService;
            _tokenService = tokenService;
        }

        /// <summary>
        /// Registers a new user (Broker or Seeker).
        /// </summary>
        [HttpPost("register")]
        [ProducesResponseType(typeof(object), 200)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> Register(string username, string email, string password, string role)
        {
            var result = await _userService.RegisterAsync(username, email, password, role);
            if (result != null)
                return Ok(result);

            return BadRequest("Registration failed");
        }

        /// <summary>
        /// Logs in an existing user and returns JWT token.
        /// </summary>
        [HttpPost("login")]
        [ProducesResponseType(typeof(object), 200)]
        [ProducesResponseType(401)]
        public async Task<IActionResult> Login(string username, string password)
        {
            var user = await _userService.LoginAsync(username, password);
            if (user == null)
                return Unauthorized("Invalid credentials");

            // Generate JWT token for API usage
            var token = await _tokenService.GenerateTokenAsync((ApplicationUser)user);
            return Ok(new { Token = token });
        }

        /// <summary>
        /// Logs out the current user (cookie-based).
        /// </summary>
        [HttpPost("logout")]
        [ProducesResponseType(200)]
        public async Task<IActionResult> Logout()
        {
            await _userService.LogoutAsync();
            return Ok("Logged out successfully");
        }

        [HttpPost]
        public async Task<IActionResult> GenerateKey()
        {
            var key = new byte[32]; // 256 bits
            key = RandomNumberGenerator.GetBytes(32);
            return Ok(new { key = key });
        }
    }
}
