using HouseBrokerApp.Application.DTOs;
using HouseBrokerApp.Application.Interfaces;
using HouseBrokerApp.Core.Enums;
using HouseBrokerApp.Infrastructure.Identity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace HouseBrokerApp.Web.ApiControllers
{
    [Route("api/listing")]
    [ApiController]
    public class ListingsApiController : ControllerBase
    {
        private readonly IListingService _listingService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IFileStorage _fileStorage;

        public ListingsApiController(
            IListingService listingService,
            UserManager<ApplicationUser> userManager,
            IFileStorage fileStorage)
        {
            _listingService = listingService;
            _userManager = userManager;
            _fileStorage = fileStorage ?? throw new ArgumentNullException(nameof(fileStorage));
        }

        /// <summary>
        /// Gets all property listings (public).
        /// </summary>
        [HttpGet]
        [AllowAnonymous]
        [ProducesResponseType(typeof(IEnumerable<PropertyListingDto>), 200)]
        public async Task<IActionResult> GetAll()
        {
            var listings = await _listingService.GetAllAsync();
            return Ok(listings);
        }

        /// <summary>
        /// Gets a property listing by its ID (public).
        /// </summary>
        [HttpGet("{id}")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(PropertyListingDto), 200)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetById(Guid id)
        {
            var listing = await _listingService.GetByIdAsync(id);
            if (listing == null) return NotFound();
            return Ok(listing);
        }

        /// <summary>
        /// Creates a new property listing (restricted to Brokers via JWT).
        /// </summary>
        [HttpPost]
        [Authorize(
            AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme,
            Roles = "Broker")]
        [ProducesResponseType(typeof(PropertyListingDto), 201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        [RequestSizeLimit(10_000_000)] // 10MB limit
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> Create([FromForm] PropertyListingDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            // Save uploaded image if provided
            if (dto.ImageFile != null)
            {
                dto.ImageUrl = await _fileStorage.SaveFileAsync(dto.ImageFile);
            }

            // Attach broker ID from logged-in user
            var user = await _userManager.GetUserAsync(User);
            dto.BrokerId = user!.Id;

            await _listingService.AddAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = dto.Id }, dto);
        }

        /// <summary>
        /// Update an existing listing (with optional image replacement).
        /// </summary>
        [HttpPut("{id:guid}")]
        [Authorize(
            AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme,
            Roles = "Broker")]
        [Consumes("multipart/form-data")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [RequestSizeLimit(10_000_000)] // 10MB limit
        public async Task<IActionResult> Update(Guid id, [FromForm] PropertyListingDto dto)
        {
            if (id != dto.Id) return BadRequest("ID mismatch");
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var existing = await _listingService.GetByIdAsync(id);
            if (existing == null) return NotFound();

            if (dto.ImageFile != null)
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
            return NoContent();
        }

        /// <summary>
        /// Delete a property listing by ID.
        /// </summary>
        [HttpDelete("{id:guid}")]
        [Authorize(
            AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme,
            Roles = "Broker")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(Guid id)
        {
            var existing = await _listingService.GetByIdAsync(id);
            if (existing == null) return NotFound();

            await _listingService.DeleteAsync(id);
            return NoContent();
        }

        /// <summary>
        /// Searches property listings (public).
        /// </summary>
        [HttpGet("search")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(IEnumerable<PropertyListingDto>), 200)]
        public async Task<IActionResult> Search([FromQuery] string? location, [FromQuery] decimal? minPrice,
                                                [FromQuery] decimal? maxPrice, [FromQuery] PropertyType? propertyType)
        {
            var results = await _listingService.SearchAsync(location, minPrice, maxPrice, propertyType);
            return Ok(results);
        }
    }
}
