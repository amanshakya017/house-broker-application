using HouseBrokerApp.Application.Interfaces;
using HouseBrokerApp.Infrastructure.Identity;
using HouseBrokerApp.Infrastructure.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace HouseBrokerApp.Infrastructure.Services
{
    /// <summary>
    /// Service responsible for handling user authentication and role management.
    /// Wraps ASP.NET Core Identity's <see cref="UserManager{TUser}"/> and <see cref="SignInManager{TUser}"/>.
    /// </summary>
    public class UserService : IUserService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public UserService(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        /// <summary>
        /// Registers a new user with the specified username, email, password, and role.
        /// </summary>
        /// <param name="username">The username of the new user.</param>
        /// <param name="email">The email of the new user.</param>
        /// <param name="password">The password for the new user.</param>
        /// <param name="role">The role to assign (e.g., "Broker" or "Seeker").</param>
        /// <returns>
        /// The created <see cref="ApplicationUser"/> if successful; otherwise, a collection of validation errors.
        /// </returns>
        public async Task<object?> RegisterAsync(string username, string email, string password, string role)
        {
            var user = new ApplicationUser
            {
                UserName = username,
                Email = email,
                Role = role,
                EmailConfirmed = true
            };

            var result = await _userManager.CreateAsync(user, password);
            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, role);
                return user;
            }
            return result.Errors;
        }

        /// <summary>
        /// Attempts to log in a user using the provided credentials.
        /// </summary>
        /// <param name="username">The username of the user.</param>
        /// <param name="password">The user's password.</param>
        /// <returns>
        /// The <see cref="ApplicationUser"/> if login succeeds; otherwise, null.
        /// </returns>
        public async Task<object?> LoginAsync(string username, string password)
        {
            var result = await _signInManager.PasswordSignInAsync(username, password, false, false);
            if (result.Succeeded)
            {
                return await _userManager.FindByNameAsync(username);
            }
            return null;
        }

        /// <summary>
        /// Logs out the currently signed-in user.
        /// </summary>
        public async Task LogoutAsync() => await _signInManager.SignOutAsync();
    }
}
