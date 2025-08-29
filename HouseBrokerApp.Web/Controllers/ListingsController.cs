using HouseBrokerApp.Application.DTOs;
using HouseBrokerApp.Application.Interfaces;
using HouseBrokerApp.Infrastructure.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace HouseBrokerApp.Web.Controllers
{
    /// <summary>
    /// Controller for managing property listings.
    /// Provides CRUD operations, search/filter functionality, and detail views.
    /// Only Brokers can create, edit, or delete listings, while Seekers can view and search.
    /// </summary>
    public class ListingsController : Controller
    {
        private readonly IListingService _listingService;
        private readonly IBrokerService _brokerService;
        private readonly UserManager<ApplicationUser> _userManager;

        public ListingsController(IListingService listingService, IBrokerService brokerService, UserManager<ApplicationUser> userManager)
        {
            _listingService = listingService;
            _brokerService = brokerService;
            _userManager = userManager;
        }

        /// <summary>
        /// Displays a searchable list of property listings.
        /// Filters can be applied for location, price range, and property type.
        /// </summary>
        /// <param name="model">Search model containing filter criteria.</param>
        /// <returns>A view showing a list of property listings.</returns>
        public async Task<IActionResult> Index(ListingSearchModel model)
        {
            model.Listings = await _listingService.SearchAsync(model.Location, model.MinPrice, model.MaxPrice, model.PropertyType);
            return View(model);
        }

        /// <summary>
        /// Displays detailed information about a single property listing.
        /// </summary>
        /// <param name="id">The unique identifier of the property listing.</param>
        /// <returns>A view with detailed listing information or NotFound if not found.</returns>
        public async Task<IActionResult> Details(Guid id)
        {
            var listing = await _listingService.GetByIdAsync(id);
            if (listing == null) return NotFound();
            return View(listing);
        }

        /// <summary>
        /// Returns the view for creating a new property listing.
        /// Accessible only to Brokers.
        /// </summary>
        [Authorize(Roles = "Broker")]
        [HttpGet]
        public IActionResult Create() => View();

        /// <summary>
        /// Handles submission of a new property listing.
        /// Assigns the listing to the currently logged-in broker.
        /// </summary>
        /// <param name="dto">The listing details submitted by the broker.</param>
        /// <returns>Redirects to Index on success; otherwise returns the same view with validation errors.</returns>
        [Authorize(Roles = "Broker")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(PropertyListingDto dto)
        {
            if (!ModelState.IsValid) return View(dto);

            if (dto.ImageFile != null && dto.ImageFile.Length > 0)
            {
                var fileName = Guid.NewGuid() + Path.GetExtension(dto.ImageFile.FileName);
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img", fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await dto.ImageFile.CopyToAsync(stream);
                }

                dto.ImageUrl = "/img/" + fileName;
            }

            var user = await _userManager.GetUserAsync(User);
            dto.BrokerId = user!.Id;

            await _listingService.AddAsync(dto);
            return RedirectToAction(nameof(Index));
        }

        /// <summary>
        /// Returns the view for editing an existing listing.
        /// Accessible only to Brokers.
        /// </summary>
        /// <param name="id">The unique identifier of the listing to edit.</param>
        /// <returns>A view with pre-filled listing data or NotFound if the listing doesn't exist.</returns>
        [Authorize(Roles = "Broker")]
        [HttpGet]
        public async Task<IActionResult> Edit(Guid id)
        {
            var dto = await _listingService.GetByIdAsync(id);
            if (dto == null) return NotFound();

            return View(dto);
        }

        /// <summary>
        /// Handles submission of an updated listing.
        /// </summary>
        /// <param name="dto">The updated listing details.</param>
        /// <returns>Redirects to Index on success; otherwise returns the same view with validation errors.</returns>
        [Authorize(Roles = "Broker")]
        [HttpPost]
        public async Task<IActionResult> Edit(PropertyListingDto dto)
        {
            if (!ModelState.IsValid) return View(dto);
            if (dto.ImageFile != null && dto.ImageFile.Length > 0)
            {
                var fileName = Guid.NewGuid() + Path.GetExtension(dto.ImageFile.FileName);
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img", fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await dto.ImageFile.CopyToAsync(stream);
                }

                dto.ImageUrl = "/img/" + fileName;
            }
            await _listingService.UpdateAsync(dto);
            return RedirectToAction(nameof(Index));
        }

        /// <summary>
        /// Displays a confirmation view for deleting a listing.
        /// </summary>
        /// <param name="id">The unique identifier of the listing to delete.</param>
        /// <returns>A view with listing details for confirmation or NotFound if not found.</returns>
        [Authorize(Roles = "Broker")]
        [HttpGet]
        public async Task<IActionResult> Delete(Guid id)
        {
            var dto = await _listingService.GetByIdAsync(id);
            if (dto == null) return NotFound();

            return View(dto);
        }

        /// <summary>
        /// Handles confirmation of listing deletion.
        /// </summary>
        /// <param name="id">The unique identifier of the listing to delete.</param>
        /// <returns>Redirects to Index after deletion.</returns>
        [Authorize(Roles = "Broker")]
        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            await _listingService.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
