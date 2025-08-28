using System.ComponentModel.DataAnnotations;

namespace HouseBrokerApp.Application.DTOs
{
    /// <summary>
    /// Data Transfer Object (DTO) for property listings.
    /// Used to transfer property data between layers (API, Services, and UI).
    /// Includes essential details such as type, location, features, broker info, and commission.
    /// </summary>
    public class PropertyListingDto
    {
        /// <summary>
        /// Unique identifier of the property listing.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Type of property (e.g., House, Apartment, Land).
        /// </summary>
        [Required(ErrorMessage = "Property type is required")]
        [StringLength(100, ErrorMessage = "Property type cannot exceed 100 characters.")]
        public string PropertyType { get; set; } = string.Empty;

        /// <summary>
        /// Location of the property (city, district, or detailed address).
        /// </summary>
        [Required(ErrorMessage = "Location is required")]
        [StringLength(200, ErrorMessage = "Location cannot exceed 200 characters.")]
        public string Location { get; set; } = string.Empty;

        /// <summary>
        /// Price of the property in local currency (NPR).
        /// Must be between 1 Lakh and 10 Crore.
        /// </summary>
        [Range(100000, 100000000, ErrorMessage = "Price must be between 1 Lakh and 10 Crore")]
        public decimal Price { get; set; }

        /// <summary>
        /// Key features of the property (e.g., "3BHK, Swimming Pool, Parking").
        /// </summary>
        [Required(ErrorMessage = "Features are required")]
        public string Features { get; set; } = string.Empty;

        /// <summary>
        /// Detailed description of the property (maximum 1000 characters).
        /// </summary>
        [Required(ErrorMessage = "Description is required")]
        [StringLength(1000, ErrorMessage = "Description cannot exceed 1000 characters.")]
        public string Description { get; set; } = string.Empty;

        /// <summary>
        /// URL of the property image (must be a valid HTTP/HTTPS link).
        /// </summary>
        [Url(ErrorMessage = "Image URL must be valid.")]
        public string ImageUrl { get; set; } = string.Empty;

        /// <summary>
        /// Unique identifier of the broker who owns the listing.
        /// </summary>
        public Guid BrokerId { get; set; }

        /// <summary>
        /// Commission amount calculated based on property price and commission rules.
        /// </summary>
        public decimal Commission { get; set; }
    }
}
