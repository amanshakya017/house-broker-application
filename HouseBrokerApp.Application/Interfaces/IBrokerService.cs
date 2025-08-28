using HouseBrokerApp.Core.Entities;

namespace HouseBrokerApp.Application.Interfaces
{
    public interface IBrokerService
    {
        Task<IEnumerable<PropertyListing>> GetBrokerListingsAsync(Guid brokerId);
        Task<decimal> GetTotalCommissionAsync(Guid brokerId);
        Task<decimal> CalculateCommissionAsync(decimal price);
    }
}
