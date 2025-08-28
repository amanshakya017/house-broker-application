using HouseBrokerApp.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace HouseBrokerApp.Web.ApiControllers
{
    [Route("api/broker")]
    [ApiController]
    [Authorize(
        AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme,
        Roles = "Broker")]
    public class BrokerApiController : ControllerBase
    {
        private readonly IBrokerService _brokerService;

        public BrokerApiController(IBrokerService brokerService)
        {
            _brokerService = brokerService;
        }

        /// <summary>
        /// Gets all property listings for a broker.
        /// </summary>
        [HttpGet("listings/{brokerId}")]
        [ProducesResponseType(typeof(IEnumerable<object>), 200)]
        public async Task<IActionResult> GetBrokerListings(Guid brokerId)
        {
            var listings = await _brokerService.GetBrokerListingsAsync(brokerId);
            return Ok(listings);
        }

        /// <summary>
        /// Gets the total commission earned by a broker.
        /// </summary>
        [HttpGet("commission/{brokerId}")]
        [ProducesResponseType(typeof(decimal), 200)]
        public async Task<IActionResult> GetTotalCommission(Guid brokerId)
        {
            var total = await _brokerService.GetTotalCommissionAsync(brokerId);
            return Ok(total);
        }

        /// <summary>
        /// Calculates commission for a given property price.
        /// </summary>
        [HttpGet("commission/calc")]
        [ProducesResponseType(typeof(decimal), 200)]
        public async Task<IActionResult> CalculateCommission([FromQuery] decimal price)
        {
            var commission = await _brokerService.CalculateCommissionAsync(price);
            return Ok(commission);
        }
    }
}
