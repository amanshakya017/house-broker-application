using HouseBrokerApp.Application.DTOs;

namespace HouseBrokerApp.Application.Interfaces
{
    public interface IListingService
    {
        Task<IEnumerable<PropertyListingDto>> GetAllAsync();
        Task<PropertyListingDto?> GetByIdAsync(Guid id);
        Task AddAsync(PropertyListingDto dto);
        Task UpdateAsync(PropertyListingDto dto);
        Task DeleteAsync(Guid id);
        Task<IEnumerable<PropertyListingDto>> SearchAsync(string? location, decimal? minPrice, decimal? maxPrice, string? propertyType);
    }
}
