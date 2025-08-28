namespace HouseBrokerApp.Application.DTOs
{
    /// <summary>
    /// Data Transfer Object (DTO) and ViewModel used for property listing search functionality.
    /// Holds search filters and the resulting list of property listings.
    /// </summary>
    public class ListingSearchModel
    {
        /// <summary>
        /// Location filter for searching property listings (optional).
        /// </summary>
        public string? Location { get; set; }

        /// <summary>
        /// Minimum price filter for property listings (optional).
        /// </summary>
        public decimal? MinPrice { get; set; }

        /// <summary>
        /// Maximum price filter for property listings (optional).
        /// </summary>
        public decimal? MaxPrice { get; set; }

        /// <summary>
        /// Property type filter (e.g., "House", "Apartment", "Land") (optional).
        /// </summary>
        public string? PropertyType { get; set; }

        /// <summary>
        /// A collection of property listings that match the applied search filters.
        /// </summary>
        public IEnumerable<PropertyListingDto> Listings { get; set; } = new List<PropertyListingDto>();
    }
}
