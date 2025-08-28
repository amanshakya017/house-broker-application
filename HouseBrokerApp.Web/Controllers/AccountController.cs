using HouseBrokerApp.Infrastructure.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace HouseBrokerApp.Web.Controllers
{
    /// <summary>
    /// Controller responsible for handling user account operations such as login, registration, and logout.
    /// Uses <see cref="IUserService"/> to wrap ASP.NET Identity logic.
    /// </summary>
    public class AccountController : Controller
    {
        private readonly IUserService _userService;

        public AccountController(IUserService userService)
        {
            _userService = userService;
        }

        /// <summary>
        /// Returns the login view where users can enter their credentials.
        /// </summary>
        [HttpGet]
        public IActionResult Login() => View();

        /// <summary>
        /// Handles login form submission.
        /// If credentials are valid, the user is redirected to the home page.
        /// </summary>
        /// <param name="username">The username of the user attempting to log in.</param>
        /// <param name="password">The password of the user.</param>
        /// <returns>
        /// Redirects to Home/Index on success; otherwise redisplays the login page with an error message.
        /// </returns>
        [HttpPost]
        public async Task<IActionResult> Login(string username, string password)
        {
            var result = await _userService.LoginAsync(username, password);
            if (result != null)
                return RedirectToAction("Index", "Home");

            ViewBag.Error = "Invalid username or password";
            return View();
        }

        /// <summary>
        /// Returns the registration view where new users can sign up.
        /// </summary>
        [HttpGet]
        public IActionResult Register() => View();

        /// <summary>
        /// Handles registration form submission.
        /// Registers a new user with the given details and role.
        /// </summary>
        /// <param name="username">Desired username.</param>
        /// <param name="email">Email address.</param>
        /// <param name="password">Account password.</param>
        /// <param name="role">User role (e.g., Broker or Seeker).</param>
        /// <returns>
        /// Redirects to the Login page if registration is successful; otherwise redisplays the form with an error.
        /// </returns>
        [HttpPost]
        public async Task<IActionResult> Register(string username, string email, string password, string role)
        {
            var result = await _userService.RegisterAsync(username, email, password, role);
            if (result != null)
                return RedirectToAction("Login");

            ViewBag.Error = "Registration failed";
            return View();
        }

        /// <summary>
        /// Logs out the currently signed-in user.
        /// </summary>
        /// <returns>Redirects to the Login page after logging out.</returns>
        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await _userService.LogoutAsync();
            return RedirectToAction("Login");
        }
    }
}
