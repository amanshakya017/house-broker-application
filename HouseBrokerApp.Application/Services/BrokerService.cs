using HouseBrokerApp.Application.Interfaces;
using HouseBrokerApp.Core.Entities;
using HouseBrokerApp.Core.Interfaces;

namespace HouseBrokerApp.Application.Services
{
    /// <summary>
    /// Service responsible for broker-specific operations.
    /// Provides commission calculation and retrieval of broker-owned listings.
    /// </summary>
    public class BrokerService : IBrokerService
    {
        private readonly IUnitOfWork _unitOfWork;

        public BrokerService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        /// <summary>
        /// Retrieves all property listings owned by a specific broker.
        /// </summary>
        /// <param name="brokerId">The unique identifier of the broker.</param>
        /// <returns>A collection of <see cref="PropertyListing"/> belonging to the broker.</returns>
        public async Task<IEnumerable<PropertyListing>> GetBrokerListingsAsync(Guid brokerId)
        {
            return await _unitOfWork.Repository<PropertyListing>().FindAsync(l => l.BrokerId == brokerId);
        }

        /// <summary>
        /// Calculates the total commission earned by a broker from all their listings.
        /// </summary>
        /// <param name="brokerId">The unique identifier of the broker.</param>
        /// <returns>The sum of commissions from all broker-owned listings.</returns>
        public async Task<decimal> GetTotalCommissionAsync(Guid brokerId)
        {
            var listings = await GetBrokerListingsAsync(brokerId);
            return listings.Sum(l => l.Commission);
        }

        /// <summary>
        /// Calculates commission for a given property price using commission rules stored in the database.
        /// </summary>
        /// <param name="price">The property price.</param>
        /// <returns>The calculated commission amount. Returns 0 if no rule matches.</returns>
        public async Task<decimal> CalculateCommissionAsync(decimal price)
        {
            var rules = await _unitOfWork.Repository<CommissionRule>().GetAllAsync();
            var rule = rules.FirstOrDefault(r => price >= r.MinPrice && price <= r.MaxPrice);

            if (rule == null) return 0;

            return price * rule.Rate;
        }
    }
}
