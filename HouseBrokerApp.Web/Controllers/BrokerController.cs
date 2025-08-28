using HouseBrokerApp.Application.Interfaces;
using HouseBrokerApp.Infrastructure.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace HouseBrokerApp.Web.Controllers
{
    /// <summary>
    /// Controller for broker-specific functionality.
    /// Accessible only to users in the "Broker" role.
    /// Provides a dashboard view where brokers can see their listings and total earned commission.
    /// </summary>
    [Authorize(Roles = "Broker")]
    public class BrokerController : Controller
    {
        private readonly IBrokerService _brokerService;
        private readonly UserManager<ApplicationUser> _userManager;

        public BrokerController(IBrokerService brokerService, UserManager<ApplicationUser> userManager)
        {
            _brokerService = brokerService;
            _userManager = userManager;
        }

        /// <summary>
        /// Displays the broker's dashboard containing:
        /// - A list of all property listings owned by the broker.
        /// - The total commission earned across all their listings.
        /// </summary>
        /// <returns>
        /// A view displaying broker-owned listings and total commission, 
        /// or <see cref="UnauthorizedResult"/> if the broker is not logged in.
        /// </returns>
        [HttpGet]
        public async Task<IActionResult> Dashboard()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Unauthorized();

            var listings = await _brokerService.GetBrokerListingsAsync(user.Id);
            var totalCommission = await _brokerService.GetTotalCommissionAsync(user.Id);

            ViewBag.TotalCommission = totalCommission;
            return View(listings);
        }
    }
}
