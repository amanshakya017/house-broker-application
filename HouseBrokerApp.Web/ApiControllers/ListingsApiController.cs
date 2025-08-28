using HouseBrokerApp.Application.DTOs;
using HouseBrokerApp.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace HouseBrokerApp.Web.ApiControllers
{
    [Route("api/listing")]
    [ApiController]
    public class ListingsApiController : ControllerBase
    {
        private readonly IListingService _listingService;

        public ListingsApiController(IListingService listingService)
        {
            _listingService = listingService;
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
        public async Task<IActionResult> Create([FromBody] PropertyListingDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            await _listingService.AddAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = dto.Id }, dto);
        }

        /// <summary>
        /// Searches property listings (public).
        /// </summary>
        [HttpGet("search")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(IEnumerable<PropertyListingDto>), 200)]
        public async Task<IActionResult> Search([FromQuery] string? location, [FromQuery] decimal? minPrice,
                                                [FromQuery] decimal? maxPrice, [FromQuery] string? propertyType)
        {
            var results = await _listingService.SearchAsync(location, minPrice, maxPrice, propertyType);
            return Ok(results);
        }
    }
}
