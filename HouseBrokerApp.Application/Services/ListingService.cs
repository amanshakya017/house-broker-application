using HouseBrokerApp.Application.DTOs;
using HouseBrokerApp.Application.Interfaces;
using HouseBrokerApp.Core.Entities;
using HouseBrokerApp.Core.Enums;
using HouseBrokerApp.Core.Interfaces;
using Microsoft.Extensions.Caching.Memory;

namespace HouseBrokerApp.Application.Services
{
    /// <summary>
    /// Service responsible for managing property listings.
    /// Provides CRUD operations, caching, commission calculation, and search functionality.
    /// </summary>
    public class ListingService : IListingService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMemoryCache _cache;

        private const string CacheKey = "PropertyListings";

        /// <summary>
        /// Service responsible for managing property listings.
        /// Provides CRUD operations, caching, commission calculation, and search functionality.
        /// </summary>
        public ListingService(IUnitOfWork unitOfWork, IMemoryCache cache)
        {
            _unitOfWork = unitOfWork;
            _cache = cache;
        }

        /// <summary>
        /// Retrieves all property listings. Uses cache for faster retrieval.
        /// </summary>
        /// <returns>A collection of <see cref="PropertyListingDto"/>.</returns>
        public async Task<IEnumerable<PropertyListingDto>> GetAllAsync()
        {
            if (!_cache.TryGetValue(CacheKey, out IEnumerable<PropertyListingDto>? cachedListings))
            {
                var listings = await _unitOfWork.Repository<PropertyListing>().GetAllAsync();

                cachedListings = listings.Select(l => new PropertyListingDto
                {
                    Id = l.Id,
                    PropertyType = l.PropertyType,
                    Location = l.Location,
                    Price = l.Price,
                    Features = l.Features,
                    Description = l.Description,
                    ImageUrl = l.ImageUrl,
                    BrokerId = l.BrokerId,
                    Commission = l.Commission
                }).ToList();

                _cache.Set(CacheKey, cachedListings, TimeSpan.FromMinutes(5));
            }

            return cachedListings!;
        }

        /// <summary>
        /// Retrieves a single property listing by ID.
        /// </summary>
        /// <param name="id">Unique identifier of the property listing.</param>
        /// <returns>The matching <see cref="PropertyListingDto"/> or null if not found.</returns>
        public async Task<PropertyListingDto?> GetByIdAsync(Guid id)
        {
            var listing = await _unitOfWork.Repository<PropertyListing>().GetByIdAsync(id);
            if (listing == null) return null;

            return new PropertyListingDto
            {
                Id = listing.Id,
                PropertyType = listing.PropertyType,
                Location = listing.Location,
                Price = listing.Price,
                Features = listing.Features,
                Description = listing.Description,
                ImageUrl = listing.ImageUrl,
                BrokerId = listing.BrokerId,
                Commission = listing.Commission
            };
        }

        /// <summary>
        /// Adds a new property listing and calculates its commission.
        /// </summary>
        /// <param name="dto">The listing DTO containing property details.</param>
        public async Task AddAsync(PropertyListingDto dto)
        {
            var entity = new PropertyListing
            {
                Id = Guid.NewGuid(),
                PropertyType = dto.PropertyType,
                Location = dto.Location,
                Price = dto.Price,
                Features = dto.Features,
                Description = dto.Description,
                ImageUrl = dto.ImageUrl,
                BrokerId = dto.BrokerId,
                Commission = CalculateCommission(dto.Price)
            };

            await _unitOfWork.Repository<PropertyListing>().AddAsync(entity);
            await _unitOfWork.SaveChangesAsync();
            _cache.Remove(CacheKey);
        }

        /// <summary>
        /// Updates an existing property listing and recalculates its commission.
        /// </summary>
        /// <param name="dto">The updated listing DTO.</param>
        public async Task UpdateAsync(PropertyListingDto dto)
        {
            if (dto.Id.HasValue)
            {
                var entity = await _unitOfWork.Repository<PropertyListing>().GetByIdAsync(dto.Id.Value);
                if (entity == null) return;

                entity.PropertyType = dto.PropertyType;
                entity.Location = dto.Location;
                entity.Price = dto.Price;
                entity.Features = dto.Features;
                entity.Description = dto.Description;
                entity.ImageUrl = dto.ImageUrl;
                entity.BrokerId = dto.BrokerId;
                entity.Commission = CalculateCommission(dto.Price);

                _unitOfWork.Repository<PropertyListing>().Update(entity);
                await _unitOfWork.SaveChangesAsync();
                _cache.Remove(CacheKey); 
            }
        }

        /// <summary>
        /// Deletes a property listing by ID.
        /// </summary>
        /// <param name="id">Unique identifier of the listing to delete.</param>
        public async Task DeleteAsync(Guid id)
        {
            var entity = await _unitOfWork.Repository<PropertyListing>().GetByIdAsync(id);
            if (entity == null) return;

            _unitOfWork.Repository<PropertyListing>().Delete(entity);
            await _unitOfWork.SaveChangesAsync();
            _cache.Remove(CacheKey);
        }

        /// <summary>
        /// Calculates commission for a given price based on predefined rules.
        /// </summary>
        /// <param name="price">The property price.</param>
        /// <returns>The calculated commission amount.</returns>
        private decimal CalculateCommission(decimal price)
        {
            if (price < 5000000)
                return price * 0.02m;
            else if (price <= 10000000)
                return price * 0.0175m;
            else
                return price * 0.015m;
        }

        /// <summary>
        /// Searches listings by location, property type, and price range.
        /// </summary>
        /// <param name="location">Filter by location (optional).</param>
        /// <param name="minPrice">Minimum price (optional).</param>
        /// <param name="maxPrice">Maximum price (optional).</param>
        /// <param name="propertyType">Filter by property type (optional).</param>
        /// <returns>A filtered collection of <see cref="PropertyListingDto"/>.</returns>
        public async Task<IEnumerable<PropertyListingDto>> SearchAsync(string? location, decimal? minPrice, decimal? maxPrice, PropertyType? propertyType)
        {
            var query = (await GetAllAsync()).AsQueryable();

            // Location filter
            if (!string.IsNullOrWhiteSpace(location))
                query = query.Where(x => x.Location.Contains(location, StringComparison.OrdinalIgnoreCase));

            // PropertyType filter (enum as int)
            if (propertyType.HasValue)
                query = query.Where(x => x.PropertyType == propertyType.Value);  // ✅ enum comparison

            // Min price filter
            if (minPrice.HasValue)
                query = query.Where(x => x.Price >= minPrice.Value);

            // Max price filter
            if (maxPrice.HasValue)
                query = query.Where(x => x.Price <= maxPrice.Value);

            return query.ToList();
        }

    }
}
